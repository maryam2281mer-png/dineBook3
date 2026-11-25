
<%@ Page Title="Contact Us" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="DineBook.Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<section class="page-section">
    <h1>Contact Us</h1>
    <p>We would love to hear from you. Send us your questions or feedback.</p>

    <div class="form-card">
        <div class="form-group">
            <label>Your Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator
                ID="valName" runat="server"
                ControlToValidate="txtName"
                ErrorMessage="Please enter your name."
                ForeColor="Red" />
        </div>

        <div class="form-group">
            <label>Email Address</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
            <asp:RequiredFieldValidator
                ID="valEmail" runat="server"
                ControlToValidate="txtEmail"
                ErrorMessage="Please enter your email."
                ForeColor="Red" />
        </div>

        <div class="form-group">
            <label>Message</label>
            <asp:TextBox ID="txtMessage" runat="server"
                CssClass="form-control" TextMode="MultiLine" Rows="5" />
            <asp:RequiredFieldValidator
                ID="valMessage" runat="server"
                ControlToValidate="txtMessage"
                ErrorMessage="Please enter your message."
                ForeColor="Red" />
        </div>

        <asp:Button ID="btnSend" runat="server"
            Text="Send Message"
            CssClass="btn-primary"
            OnClick="btnSend_Click" />

        <asp:Label ID="lblMessage" runat="server" />
    </div>
</section>

</asp:Content>
