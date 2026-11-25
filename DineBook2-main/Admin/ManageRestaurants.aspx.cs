
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DineBook.Admin
{
    public partial class ManageRestaurants : System.Web.UI.Page
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
                LoadRestaurants();
        }

        private void LoadRestaurants()
        {
            string query = @"
                SELECT ServiceID, ServiceName, Description,
                       Duration, Price, MaxGuests, ServicePhoto
                FROM Services
                ORDER BY ServiceID DESC";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvRestaurants.DataSource = dt;
                gvRestaurants.DataBind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int duration;
            int maxGuests;
            decimal price;

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                txtName.Text.Trim().Length > 150 ||
                !int.TryParse(txtMaxGuests.Text, out maxGuests) ||
                maxGuests < 1 || maxGuests > 100 ||
                !int.TryParse(txtDuration.Text, out duration) ||
                duration <= 0 ||
                !decimal.TryParse(txtPrice.Text,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out price) ||
                price < 0)
            {
                lblMessage.Text = "Enter a restaurant name, valid duration, price, and maximum guests (1-100).";
                return;
            }

            string query = @"
                INSERT INTO Services
                (ServiceName, Description, Duration, Price, MaxGuests, ServicePhoto)
                VALUES
                (@Name, @Description, @Duration, @Price, @MaxGuests, @Photo)";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value =
                    txtName.Text.Trim();

                cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value =
                    string.IsNullOrWhiteSpace(txtDescription.Text)
                    ? (object)DBNull.Value : txtDescription.Text.Trim();

                cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = duration;

                cmd.Parameters.Add("@MaxGuests", SqlDbType.Int).Value = maxGuests;

                cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = price;

                cmd.Parameters.Add("@Photo", SqlDbType.NVarChar, 500).Value =
                    string.IsNullOrWhiteSpace(txtPhoto.Text)
                    ? (object)DBNull.Value : txtPhoto.Text.Trim();

                cmd.Parameters["@Price"].Precision = 10;
                cmd.Parameters["@Price"].Scale = 2;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Restaurant added successfully.";

            txtName.Text = "";
            txtDescription.Text = "";
            txtDuration.Text = "";
            txtPrice.Text = "";
            txtMaxGuests.Text = "8";
            txtPhoto.Text = "";

            LoadRestaurants();
        }

        protected void gvRestaurants_RowEditing(object sender,
            System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvRestaurants.EditIndex = e.NewEditIndex;
            LoadRestaurants();
        }

        protected void gvRestaurants_RowCancelingEdit(object sender,
            System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvRestaurants.EditIndex = -1;
            LoadRestaurants();
        }

        protected void gvRestaurants_RowUpdating(object sender,
            System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvRestaurants.DataKeys[e.RowIndex].Value);
            var row = gvRestaurants.Rows[e.RowIndex];

            string name = ((System.Web.UI.WebControls.TextBox)
                row.Cells[1].Controls[0]).Text.Trim();

            string description = ((System.Web.UI.WebControls.TextBox)
                row.Cells[2].Controls[0]).Text.Trim();

            int duration;
            int maxGuests;
            decimal price;

            if (string.IsNullOrWhiteSpace(name) || name.Length > 150)
            {
                lblMessage.Text = "Restaurant name is required (up to 150 characters).";
                return;
            }

            if (!int.TryParse(((System.Web.UI.WebControls.TextBox)
                row.Cells[5].Controls[0]).Text, out maxGuests) ||
                maxGuests < 1 || maxGuests > 100)
            {
                lblMessage.Text = "Maximum guests must be between 1 and 100.";
                return;
            }

            if (!int.TryParse(((System.Web.UI.WebControls.TextBox)
                row.Cells[3].Controls[0]).Text, out duration) || duration <= 0 ||
                !decimal.TryParse(((System.Web.UI.WebControls.TextBox)
                row.Cells[4].Controls[0]).Text,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture, out price) ||
                price < 0)
            {
                lblMessage.Text = "Enter valid duration and price values.";
                return;
            }

            string photo = ((System.Web.UI.WebControls.TextBox)
                row.Cells[6].Controls[0]).Text.Trim();

            string query = @"
                UPDATE Services
                SET ServiceName = @Name,
                    Description = @Description,
                    Duration = @Duration,
                    Price = @Price,
                    MaxGuests = @MaxGuests,
                    ServicePhoto = @Photo
                WHERE ServiceID = @ID";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = name;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value =
                    string.IsNullOrWhiteSpace(description)
                    ? (object)DBNull.Value : description;
                cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = duration;
                cmd.Parameters.Add("@MaxGuests", SqlDbType.Int).Value = maxGuests;
                cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = price;
                cmd.Parameters["@Price"].Precision = 10;
                cmd.Parameters["@Price"].Scale = 2;
                cmd.Parameters.Add("@Photo", SqlDbType.NVarChar, 500).Value =
                    string.IsNullOrWhiteSpace(photo)
                    ? (object)DBNull.Value : photo;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            gvRestaurants.EditIndex = -1;
            LoadRestaurants();
            lblMessage.Text = "Restaurant updated successfully.";
        }

        protected void gvRestaurants_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvRestaurants.DataKeys[e.RowIndex].Value);

            string query = @"
                DELETE FROM Services
                WHERE ServiceID = @ID
                AND NOT EXISTS (
                    SELECT 1 FROM Appointments WHERE ServiceID = @ID
                )
                AND NOT EXISTS (
                    SELECT 1 FROM Availability WHERE ServiceID = @ID
                )";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                con.Open();
                int affected = cmd.ExecuteNonQuery();

                lblMessage.Text = affected > 0
                    ? "Restaurant deleted successfully."
                    : "This restaurant has reservations or availability records and cannot be deleted.";
            }

            LoadRestaurants();
        }
    }
}
