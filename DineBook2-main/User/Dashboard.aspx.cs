using System;

namespace DineBook.User
{
    public partial class Dashboard : System.Web.UI.Page
    {
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
                lblUserName.Text = Server.HtmlEncode(
                    Session["UserName"] == null
                    ? "Customer"
                    : Session["UserName"].ToString()
                );
            }
        }
    }
}
