using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DineBook.User
{
    public partial class Profile : System.Web.UI.Page
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

            if (Session["Role"] == null ||
                Session["Role"].ToString() != "User")
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadProfile();
            }
        }

        private void LoadProfile()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            string query = @"
                SELECT Name, Email, Phone
                FROM Users
                WHERE UserID = @UserID";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtName.Text = reader["Name"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        txtPhone.Text = reader["Phone"] == DBNull.Value
                            ? ""
                            : reader["Phone"].ToString();
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            int userId = Convert.ToInt32(Session["UserID"]);

            string query = @"
                UPDATE Users
                SET Name = @Name,
                    Email = @Email,
                    Phone = @Phone
                WHERE UserID = @UserID";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value =
                        txtName.Text.Trim();

                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                        txtEmail.Text.Trim();

                    cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value =
                        string.IsNullOrWhiteSpace(txtPhone.Text)
                        ? (object)DBNull.Value
                        : txtPhone.Text.Trim();

                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                Session["UserName"] = txtName.Text.Trim();
                Session["UserEmail"] = txtEmail.Text.Trim();

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Your profile has been updated successfully.";
            }
            catch (SqlException ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                if (ex.Number == 2627 || ex.Number == 2601)
                    lblMessage.Text = "This email address is already registered.";
                else
                    lblMessage.Text = "Unable to update your profile. Please try again.";
            }
        }
    }
}
