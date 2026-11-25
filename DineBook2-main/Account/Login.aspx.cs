using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DineBook.Helpers;

namespace DineBook.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Session["UserID"] != null)
            {
                RedirectByRole(Convert.ToString(Session["Role"]));
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string connectionString =
                ConfigurationManager.ConnectionStrings["DineBookConnection"].ConnectionString;

            int userId = 0;
            string name = null;
            string email = null;
            string role = null;
            string storedPassword = null;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT UserID, Name, Email, [Password], Role
                      FROM Users
                      WHERE Email = @Email", con))
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
                            role = reader["Role"].ToString();
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
                lblMessage.Text = "Invalid email or password.";
                return;
            }

            // Admin accounts must sign in from the dedicated admin page.
            // (Checked only after the password is verified, so the message
            // is never shown to someone who does not know the password.)
            if (role == "Admin")
            {
                lblMessage.Text = "Please use the admin login page. / يرجى استخدام صفحة دخول المدير";
                return;
            }

            Session.Clear();
            Session["UserID"] = userId;
            Session["UserName"] = name;
            Session["UserEmail"] = email;
            Session["Role"] = role;

            RedirectByRole(role);
        }

        private void RedirectByRole(string role)
        {
            Response.Redirect(role == "Admin"
                ? "~/Admin/Dashboard.aspx"
                : "~/User/Dashboard.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
