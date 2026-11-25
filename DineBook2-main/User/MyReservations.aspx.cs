using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace DineBook.User
{
    public partial class MyReservations : System.Web.UI.Page
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
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (Convert.ToString(Session["Role"]) != "User")
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadReservations();
            }
        }

        // Only confirmed reservations that are still in the future can be cancelled.
        protected bool CanCancel(object status, object date, object time)
        {
            if (Convert.ToString(status) != "Confirmed")
                return false;

            DateTime when = ((DateTime)date).Date + (TimeSpan)time;
            return when > DateTime.Now;
        }

        private void LoadReservations()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            string query = @"
                SELECT
                    A.AppointmentID,
                    S.ServiceName,
                    A.AppointmentDate,
                    A.AppointmentTime,
                    A.NumberOfGuests,
                    A.SpecialRequest,
                    A.Status
                FROM Appointments A
                INNER JOIN Services S ON A.ServiceID = S.ServiceID
                WHERE A.UserID = @UserID
                ORDER BY A.AppointmentDate DESC, A.AppointmentTime DESC";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvReservations.DataSource = dt;
                gvReservations.DataBind();
            }
        }

        protected void gvReservations_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "CancelReservation")
                return;

            int appointmentId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out appointmentId))
                return;

            int userId = Convert.ToInt32(Session["UserID"]);

            // The UserID condition guarantees a user can only cancel their own reservation.
            string query = @"
                UPDATE Appointments
                SET Status = 'Cancelled'
                WHERE AppointmentID = @ID
                  AND UserID = @UserID
                  AND Status = 'Confirmed'";

            int affected;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = appointmentId;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                con.Open();
                affected = cmd.ExecuteNonQuery();
            }

            LoadReservations();

            lblMessage.Text = affected > 0
                ? "Your reservation has been cancelled."
                : "This reservation could not be cancelled.";
        }
    }
}
