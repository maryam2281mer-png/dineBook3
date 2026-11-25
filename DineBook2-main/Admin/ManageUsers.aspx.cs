
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DineBook.Helpers;

namespace DineBook.Admin
{
    public partial class ManageUsers : System.Web.UI.Page
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
                LoadUsers();
        }

        private void LoadUsers()
        {
            string query = @"
                SELECT UserID, Name, Email, Phone
                FROM Users
                WHERE Role = 'User'
                ORDER BY UserID DESC";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvUsers.DataSource = dt;
                gvUsers.DataBind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblMessage.Text = "Name, email, and password are required.";
                return;
            }

            if (txtPassword.Text.Length < 8)
            {
                lblMessage.Text = "Password must be at least 8 characters long.";
                return;
            }

            string query = @"
                INSERT INTO Users (Name, Email, Phone, Password, Role)
                VALUES (@Name, @Email, @Phone, @Password, 'User')";

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

                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value =
                    PasswordHelper.Hash(txtPassword.Text);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "User added successfully.";

                    txtName.Text = "";
                    txtEmail.Text = "";
                    txtPhone.Text = "";
                    txtPassword.Text = "";

                    LoadUsers();
                }
                catch (SqlException ex)
                {
                    lblMessage.Text = ex.Number == 2627 || ex.Number == 2601
                        ? "This email is already registered."
                        : "Unable to add user.";
                }
            }
        }

        protected void gvUsers_RowEditing(object sender,
            System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvUsers.EditIndex = e.NewEditIndex;
            LoadUsers();
        }

        protected void gvUsers_RowCancelingEdit(object sender,
            System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvUsers.EditIndex = -1;
            LoadUsers();
        }

        protected void gvUsers_RowUpdating(object sender,
            System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int userId = Convert.ToInt32(gvUsers.DataKeys[e.RowIndex].Value);

            var row = gvUsers.Rows[e.RowIndex];

            string name = ((System.Web.UI.WebControls.TextBox)
                row.Cells[1].Controls[0]).Text.Trim();

            string email = ((System.Web.UI.WebControls.TextBox)
                row.Cells[2].Controls[0]).Text.Trim();

            string phone = ((System.Web.UI.WebControls.TextBox)
                row.Cells[3].Controls[0]).Text.Trim();

            if (name.Length == 0 || name.Length > 100 ||
                email.Length == 0 || email.Length > 150 || phone.Length > 20)
            {
                lblMessage.Text = "Name and email are required; check the maximum lengths.";
                return;
            }

            string query = @"
                UPDATE Users
                SET Name = @Name, Email = @Email, Phone = @Phone
                WHERE UserID = @UserID AND Role = 'User'";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email;
                cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value =
                    string.IsNullOrWhiteSpace(phone)
                    ? (object)DBNull.Value : phone;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    gvUsers.EditIndex = -1;
                    LoadUsers();
                    lblMessage.Text = "User updated successfully.";
                }
                catch (SqlException ex)
                {
                    lblMessage.Text = ex.Number == 2627 || ex.Number == 2601
                        ? "This email is already registered."
                        : "Unable to update user.";
                }
            }
        }

        protected void gvUsers_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int userId = Convert.ToInt32(gvUsers.DataKeys[e.RowIndex].Value);

            string query = @"
                DELETE FROM Users
                WHERE UserID = @UserID AND Role = 'User'
                AND NOT EXISTS (
                    SELECT 1 FROM Appointments WHERE UserID = @UserID
                )";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                con.Open();
                int affected = cmd.ExecuteNonQuery();

                lblMessage.Text = affected > 0
                    ? "User deleted successfully."
                    : "This user has reservations and cannot be deleted.";

                LoadUsers();
            }
        }
    }
}
