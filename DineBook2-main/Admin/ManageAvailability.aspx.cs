
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DineBook.Admin
{
    public partial class ManageAvailability : System.Web.UI.Page
    {
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DineBookConnection"].ConnectionString;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null ||
                Session["Role"] == null ||
                Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("~/Admin/AdminLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadRestaurants();
                LoadAvailability();
            }
        }

        private void LoadRestaurants()
        {
            string query = @"
                SELECT ServiceID, ServiceName
                FROM Services
                ORDER BY ServiceName";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                ddlRestaurant.DataSource = dt;
                ddlRestaurant.DataTextField = "ServiceName";
                ddlRestaurant.DataValueField = "ServiceID";
                ddlRestaurant.DataBind();

                ddlRestaurant.Items.Insert(0,
                    new System.Web.UI.WebControls.ListItem(
                        "-- Select Restaurant --", ""));
            }
        }

        private void LoadAvailability()
        {
            string query = @"
                SELECT
                    A.AvailabilityID,
                    S.ServiceName,
                    A.AvailableDate,
                    A.AvailableTime
                FROM Availability A
                INNER JOIN Services S
                    ON A.ServiceID = S.ServiceID
                ORDER BY A.AvailableDate, A.AvailableTime";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvAvailability.DataSource = dt;
                gvAvailability.DataBind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int serviceId;
            DateTime date;
            TimeSpan time;

            if (!int.TryParse(ddlRestaurant.SelectedValue, out serviceId) ||
                !DateTime.TryParse(txtDate.Text, out date) ||
                !TimeSpan.TryParse(txtTime.Text, out time))
            {
                lblMessage.Text = "Please select a restaurant, date, and time.";
                return;
            }

            if (date.Date < DateTime.Today ||
                (date.Date == DateTime.Today && time <= DateTime.Now.TimeOfDay) ||
                time < TimeSpan.Zero ||
                time >= TimeSpan.FromDays(1))
            {
                lblMessage.Text = "Please enter a valid future date and time.";
                return;
            }

            string query = @"
                INSERT INTO Availability
                    (ServiceID, AvailableDate, AvailableTime)
                VALUES
                    (@ServiceID, @Date, @Time)";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@ServiceID", SqlDbType.Int).Value = serviceId;
                cmd.Parameters.Add("@Date", SqlDbType.Date).Value = date.Date;
                cmd.Parameters.Add("@Time", SqlDbType.Time).Value = time;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Availability added successfully.";
                    txtDate.Text = "";
                    txtTime.Text = "";

                    LoadAvailability();
                }
                catch (SqlException ex)
                {
                    lblMessage.Text = ex.Number == 2627 || ex.Number == 2601
                        ? "This time is already available for this restaurant."
                        : "Unable to add availability.";
                }
            }
        }

        protected void gvAvailability_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(
                gvAvailability.DataKeys[e.RowIndex].Value);

            string query = @"
                DELETE FROM Availability
                WHERE AvailabilityID = @ID
                AND NOT EXISTS (
                    SELECT 1
                    FROM Appointments A
                    INNER JOIN Availability V
                        ON A.ServiceID = V.ServiceID
                        AND A.AppointmentDate = V.AvailableDate
                        AND A.AppointmentTime = V.AvailableTime
                    WHERE V.AvailabilityID = @ID
                    AND A.Status <> 'Cancelled'
                )";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                con.Open();
                int affected = cmd.ExecuteNonQuery();

                lblMessage.Text = affected > 0
                    ? "Availability deleted successfully."
                    : "This time has an active reservation and cannot be deleted.";
            }

            LoadAvailability();
        }
    }
}
