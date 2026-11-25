using System;

namespace DineBook
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool isLoggedIn = Session["UserID"] != null;
            string role = Session["Role"] as string;

            lnkLogin.Visible = !isLoggedIn;
            lnkRegister.Visible = !isLoggedIn;
            lnkLogout.Visible = isLoggedIn;

            lnkReservations.Visible = isLoggedIn && role == "User";

            // Logged-in users see their own dashboard; admins see the admin dashboard.
            lnkDashboard.Visible = isLoggedIn;
            if (role == "Admin")
            {
                lnkDashboard.NavigateUrl = "~/Admin/Dashboard.aspx";
                lnkDashboard.Text = "Admin Dashboard";
            }

            // The admin login link is only needed while logged out.
            lnkAdmin.Visible = !isLoggedIn;
        }
    }
}
