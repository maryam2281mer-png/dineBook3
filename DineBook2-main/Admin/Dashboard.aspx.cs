using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DineBook.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
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
                lblAdminName.Text = Server.HtmlEncode(
                    Session["UserName"] == null
                    ? "Admin"
                    : Session["UserName"].ToString()
                );

                LoadStatistics();
            }
        }

        private void LoadStatistics()
        {
            string query = @"
                SELECT
                    (SELECT COUNT(*) FROM Users WHERE Role = 'User'),
                    (SELECT COUNT(*) FROM Services),
                    (SELECT COUNT(*) FROM Appointments),
                    (SELECT COUNT(*) FROM Appointments
                        WHERE Status = 'Confirmed'
                        AND AppointmentDate >= CAST(GETDATE() AS DATE))";

            string connectionString =
                ConfigurationManager.ConnectionStrings["DineBookConnection"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            lblTotalUsers.Text = r.GetInt32(0).ToString();
                            lblTotalRestaurants.Text = r.GetInt32(1).ToString();
                            lblTotalReservations.Text = r.GetInt32(2).ToString();
                            lblUpcoming.Text = r.GetInt32(3).ToString();
                        }
                    }
                }
            }
            catch (SqlException)
            {
                // Statistics are optional; keep the dashboard usable if the query fails.
            }
        }
    }
}
