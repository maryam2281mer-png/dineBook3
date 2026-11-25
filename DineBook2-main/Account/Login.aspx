<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Login.aspx.cs"
    Inherits="DineBook.Account.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<section class="page-section">
    <div class="container" style="max-width:500px;">

        <div class="card">
            <h1 class="section-title">Welcome Back</h1>
            <p>Log in to manage your reservations.</p>

            <asp:Label ID="lblMessage" runat="server" CssClass="msg-error" />

            <asp:Label runat="server" Text="Email" />
            <asp:TextBox ID="txtEmail" runat="server"
                TextMode="Email" CssClass="form-control" MaxLength="150" />

            <asp:RequiredFieldValidator runat="server"
                ControlToValidate="txtEmail"
                Display="Dynamic"
                ErrorMessage="Please enter your email."
                ForeColor="Red" />

            <asp:Label runat="server" Text="Password" />
            <asp:TextBox ID="txtPassword" runat="server"
                TextMode="Password" CssClass="form-control" />

            <asp:RequiredFieldValidator runat="server"
                ControlToValidate="txtPassword"
                Display="Dynamic"
                ErrorMessage="Please enter your password."
                ForeColor="Red" />

            <asp:Button ID="btnLogin" runat="server"
                Text="LOG IN"
                CssClass="btn-primary"
                OnClick="btnLogin_Click" />

            <p style="margin-top:20px;">
                Don't have an account?
                <a href="Register.aspx">Create one</a>
            </p>
        </div>

    </div>
</section>

</asp:Content>
