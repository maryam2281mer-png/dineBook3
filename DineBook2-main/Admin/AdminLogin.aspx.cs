using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DineBook.Helpers;

namespace DineBook.Admin
{
    public partial class AdminLogin : System.Web.UI.Page
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
            if (!IsPostBack &&
                Session["Role"] != null &&
                Session["Role"].ToString() == "Admin")
            {
                Response.Redirect("~/Admin/Dashboard.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            int userId = 0;
            string name = null;
            string email = null;
            string storedPassword = null;

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT UserID, Name, Email, [Password]
                      FROM Users
                      WHERE Email = @Email AND Role = 'Admin'", con))
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                        txtEmail.Text.Trim();

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userId = Convert.ToInt32(reader["UserID"]);
                            name = reader["Name"].ToString();
                            email = reader["Email"].ToString();
                            storedPassword = reader["Password"].ToString();
                        }
                    }
                }
            }
            catch (SqlException)
            {
                lblMessage.Text = "Unable to reach the database. Please try again later.";
                return;
            }

            if (userId == 0 || !PasswordHelper.Verify(txtPassword.Text, storedPassword))
            {
                lblMessage.Text = "Invalid admin email or password.";
                return;
            }

            Session.Clear();
            Session["UserID"] = userId;
            Session["UserName"] = name;
            Session["UserEmail"] = email;
            Session["Role"] = "Admin";

            Response.Redirect("~/Admin/Dashboard.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
