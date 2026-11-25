
<%@ Page Title="Admin Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminLogin.aspx.cs" Inherits="DineBook.Admin.AdminLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-section">
        <div class="form-card">
            <h1>Admin Login</h1>
            <p>Sign in to manage DineBook.</p>

            <div class="form-group">
                <label>Email Address</label>
                <asp:TextBox ID="txtEmail" runat="server"
                    TextMode="Email" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Password</label>
                <asp:TextBox ID="txtPassword" runat="server"
                    TextMode="Password" CssClass="form-control" />
            </div>

            <asp:Button ID="btnLogin" runat="server"
                Text="Sign In"
                CssClass="btn-primary"
                OnClick="btnLogin_Click" />

            <br /><br />
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
        </div>
    </section>
</asp:Content>
