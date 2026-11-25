
using System;

namespace DineBook
{
    public partial class Contact : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                lblMessage.Text = "Thank you! Your message has been received.";
                lblMessage.ForeColor = System.Drawing.Color.Green;

                txtName.Text = "";
                txtEmail.Text = "";
                txtMessage.Text = "";
            }
        }
    }
}
