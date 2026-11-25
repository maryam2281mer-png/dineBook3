using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using DineBook.Helpers;

namespace DineBook.Account
{
    public partial class Register : System.Web.UI.Page
    {
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Text;

            if (name.Length == 0 || name.Length > 100)
            {
                ShowError("Please enter a valid name (up to 100 characters).");
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowError("Please enter a valid email address.");
                return;
            }

            if (phone.Length > 20)
            {
                ShowError("Phone number must be 20 characters or fewer.");
                return;
            }

            if (password.Length < 8)
            {
                ShowError("Password must be at least 8 characters long.");
                return;
            }

            string connectionString =
                ConfigurationManager.ConnectionStrings["DineBookConnection"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO Users (Name, Email, [Password], Phone, Role)
                      VALUES (@Name, @Email, @Password, @Phone, 'User')", con))
                {
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value =
                        PasswordHelper.Hash(password);
                    cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value =
                        phone.Length == 0 ? (object)DBNull.Value : phone;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMessage.Text = "Account created successfully! You can now log in.";
                lblMessage.ForeColor = System.Drawing.Color.Green;

                txtName.Text = "";
                txtEmail.Text = "";
                txtPhone.Text = "";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    ShowError("This email is already registered.");
                else
                    ShowError("Database error. Please try again.");
            }
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = System.Drawing.Color.Red;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                MailAddress address = new MailAddress(email);
                return address.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
