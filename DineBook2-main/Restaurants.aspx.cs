using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DineBook
{
    public partial class Restaurants : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRestaurants();
            }
        }

        private void LoadRestaurants()
        {
            string cs = ConfigurationManager
                .ConnectionStrings["DineBookConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT ServiceID, ServiceName,
                    Description, Duration, Price, MaxGuests, ServicePhoto
                    FROM Services
                    ORDER BY ServiceName";

                using (SqlDataAdapter adapter =
                    new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    rptRestaurants.DataSource = dt;
                    rptRestaurants.DataBind();

                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.Text =
                            "No restaurants available at the moment.";
                    }
                }
            }
        }
    }
}
