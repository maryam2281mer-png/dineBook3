
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DineBook.Admin
{
    public partial class ManageReservations : System.Web.UI.Page
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
                LoadReservations();
        }

        private void LoadReservations()
        {
            string query = @"
                SELECT
                    A.AppointmentID,
                    U.Name AS CustomerName,
                    S.ServiceName AS RestaurantName,
                    A.AppointmentDate,
                    A.AppointmentTime,
                    A.NumberOfGuests,
                    A.Status
                FROM Appointments A
                INNER JOIN Users U ON A.UserID = U.UserID
                INNER JOIN Services S ON A.ServiceID = S.ServiceID
                ORDER BY A.AppointmentDate DESC, A.AppointmentTime DESC";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvReservations.DataSource = dt;
                gvReservations.DataBind();
            }
        }

        protected void gvReservations_RowEditing(object sender,
            System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvReservations.EditIndex = e.NewEditIndex;
            LoadReservations();
        }

        protected void gvReservations_RowCancelingEdit(object sender,
            System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvReservations.EditIndex = -1;
            LoadReservations();
        }

        protected void gvReservations_RowUpdating(object sender,
            System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(
                gvReservations.DataKeys[e.RowIndex].Value);

            string status = ((System.Web.UI.WebControls.TextBox)
                gvReservations.Rows[e.RowIndex].Cells[6].Controls[0])
                .Text.Trim();

            if (status != "Confirmed" &&
                status != "Cancelled" &&
                status != "Completed")
            {
                lblMessage.Text =
                    "Status must be Confirmed, Cancelled, or Completed.";
                return;
            }

            string query = @"
                UPDATE Appointments
                SET Status = @Status
                WHERE AppointmentID = @ID";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = status;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                // The unique index UX_Appointments_ActiveSlot blocks re-activating a
                // cancelled reservation whose time was booked again in the meantime.
                lblMessage.Text = (ex.Number == 2601 || ex.Number == 2627)
                    ? "Cannot change status: another active reservation already uses this restaurant, date and time."
                    : "Unable to update the reservation.";
                return;
            }

            gvReservations.EditIndex = -1;
            LoadReservations();

            lblMessage.Text = "Reservation updated successfully.";
        }

        protected void gvReservations_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(
                gvReservations.DataKeys[e.RowIndex].Value);

            string query = @"
                DELETE FROM Appointments
                WHERE AppointmentID = @ID";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            LoadReservations();
            lblMessage.Text = "Reservation deleted successfully.";
        }
    }
}
