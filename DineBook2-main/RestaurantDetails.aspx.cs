using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace DineBook
{
    public partial class RestaurantDetails : System.Web.UI.Page
    {
        private string ConnectionString =>
            ConfigurationManager.ConnectionStrings[
                "DineBookConnection"].ConnectionString;

        private int ServiceID
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id)
                    ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ServiceID == 0)
            {
                Response.Redirect("~/Restaurants.aspx");
                return;
            }

            if (!IsPostBack)
                LoadRestaurant();
        }

        private void LoadRestaurant()
        {
            bool found = false;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT ServiceName, Description, Duration, Price,
                         MaxGuests, ServicePhoto
                  FROM Services
                  WHERE ServiceID = @ID", con))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ServiceID;
                con.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        found = true;

                        lblName.Text = Server.HtmlEncode(r["ServiceName"].ToString());
                        lblDescription.Text = Server.HtmlEncode(r["Description"].ToString());
                        lblPrice.Text = Convert.ToDecimal(r["Price"]).ToString("N2");
                        lblDuration.Text = r["Duration"].ToString();

                        string photo = r["ServicePhoto"].ToString();
                        imgRestaurant.ImageUrl = photo;
                        imgRestaurant.Visible = photo.Length > 0;
                        imgRestaurant.AlternateText = lblName.Text;

                        int maxGuests = Convert.ToInt32(r["MaxGuests"]);
                        ddlGuests.Items.Clear();
                        for (int i = 1; i <= maxGuests; i++)
                        {
                            ddlGuests.Items.Add(new ListItem(
                                i == 1 ? "1 Person" : i + " People",
                                i.ToString()));
                        }
                    }
                }
            }

            if (!found)
                Response.Redirect("~/Restaurants.aspx");
        }

        protected void btnLoadTimes_Click(object sender, EventArgs e)
        {
            DateTime selectedDate;

            if (!DateTime.TryParseExact(
                txtDate.Text, "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out selectedDate)
                || selectedDate.Date < DateTime.Today)
            {
                ShowMessage("Please choose a valid future date.", false);
                return;
            }

            ddlTime.Items.Clear();
            ddlTime.Items.Add(new ListItem("Choose a time", ""));

            // Only slots offered by the restaurant (Availability) that do not
            // already have an active (non-cancelled) reservation.
            string query = @"
                SELECT V.AvailableTime
                FROM Availability V
                WHERE V.ServiceID = @ID
                  AND V.AvailableDate = @Date
                  AND NOT EXISTS (
                      SELECT 1
                      FROM Appointments A
                      WHERE A.ServiceID = V.ServiceID
                        AND A.AppointmentDate = V.AvailableDate
                        AND A.AppointmentTime = V.AvailableTime
                        AND A.Status <> 'Cancelled')
                ORDER BY V.AvailableTime";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ServiceID;
                cmd.Parameters.Add("@Date", SqlDbType.Date).Value = selectedDate.Date;

                con.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        TimeSpan time = (TimeSpan)r["AvailableTime"];

                        // Do not offer times that already passed today.
                        if (selectedDate.Date == DateTime.Today &&
                            time <= DateTime.Now.TimeOfDay)
                            continue;

                        ddlTime.Items.Add(time.ToString(@"hh\:mm"));
                    }
                }
            }

            if (ddlTime.Items.Count == 1)
                ShowMessage("No available times for this date.", false);
            else
                ShowMessage("Available times loaded. Choose a time and confirm.", true);
        }

        protected void btnBook_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (Convert.ToString(Session["Role"]) != "User")
            {
                ShowMessage("Administrator accounts cannot make reservations.", false);
                return;
            }

            DateTime date;
            TimeSpan time;
            int guests;

            if (!DateTime.TryParseExact(
                txtDate.Text, "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out date)
                || date.Date < DateTime.Today
                || !TimeSpan.TryParse(ddlTime.SelectedValue, CultureInfo.InvariantCulture, out time)
                || !int.TryParse(ddlGuests.SelectedValue, out guests))
            {
                ShowMessage("Please select a valid date, time and guest count.", false);
                return;
            }

            if (date.Date == DateTime.Today && time <= DateTime.Now.TimeOfDay)
            {
                ShowMessage("This time has already passed.", false);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    // 1) The slot must exist in Availability for this restaurant,
                    //    and the guest count must respect the restaurant's limit.
                    int maxGuests;

                    using (SqlCommand check = new SqlCommand(
                        @"SELECT S.MaxGuests
                          FROM Services S
                          INNER JOIN Availability V ON V.ServiceID = S.ServiceID
                          WHERE S.ServiceID = @ID
                            AND V.AvailableDate = @Date
                            AND V.AvailableTime = @Time", con))
                    {
                        check.Parameters.Add("@ID", SqlDbType.Int).Value = ServiceID;
                        check.Parameters.Add("@Date", SqlDbType.Date).Value = date.Date;
                        check.Parameters.Add("@Time", SqlDbType.Time).Value = time;

                        object result = check.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            ShowMessage("This time is no longer available.", false);
                            return;
                        }

                        maxGuests = Convert.ToInt32(result);
                    }

                    if (guests < 1 || guests > maxGuests)
                    {
                        ShowMessage("This restaurant accepts 1 to " + maxGuests +
                            " guests per reservation.", false);
                        return;
                    }

                    // 2) Insert. The filtered unique index UX_Appointments_ActiveSlot
                    //    guarantees that two active reservations can never share the
                    //    same restaurant/date/time, even with simultaneous requests.
                    using (SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO Appointments
                            (UserID, ServiceID, AppointmentDate, AppointmentTime,
                             NumberOfGuests, SpecialRequest, Status)
                          VALUES
                            (@UserID, @ServiceID, @Date, @Time,
                             @Guests, @Request, 'Confirmed')", con))
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value =
                            Convert.ToInt32(Session["UserID"]);
                        cmd.Parameters.Add("@ServiceID", SqlDbType.Int).Value = ServiceID;
                        cmd.Parameters.Add("@Date", SqlDbType.Date).Value = date.Date;
                        cmd.Parameters.Add("@Time", SqlDbType.Time).Value = time;
                        cmd.Parameters.Add("@Guests", SqlDbType.Int).Value = guests;
                        cmd.Parameters.Add("@Request", SqlDbType.NVarChar, 500).Value =
                            string.IsNullOrWhiteSpace(txtRequest.Text)
                                ? (object)DBNull.Value
                                : txtRequest.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                    ShowMessage("Sorry, this time was just booked by someone else. Please choose another time.", false);
                else
                    ShowMessage("We could not complete your reservation. Please try again.", false);
                return;
            }

            Response.Redirect("~/User/MyReservations.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowMessage(string message, bool success)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.CssClass = success ? "msg-success" : "msg-error";
        }
    }
}
