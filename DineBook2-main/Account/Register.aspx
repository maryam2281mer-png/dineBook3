<%@ Page Title="Create Account" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Register.aspx.cs"
    Inherits="DineBook.Account.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<section class="page-section">
    <div class="container" style="max-width:500px;">

        <div class="card">
            <h1 class="section-title">Create Your Account</h1>
            <p>Join DineBook and start your culinary journey.</p>

            <asp:Label ID="lblMessage" runat="server" />

            <asp:Label runat="server" Text="Full Name" />
            <asp:TextBox ID="txtName" runat="server"
                CssClass="form-control" MaxLength="100" />

            <asp:RequiredFieldValidator runat="server"
                ControlToValidate="txtName"
                ErrorMessage="Please enter your name."
                ForeColor="Red" />

            <asp:Label runat="server" Text="Email" />
            <asp:TextBox ID="txtEmail" runat="server"
                TextMode="Email" CssClass="form-control" MaxLength="150" />

            <asp:RequiredFieldValidator runat="server"
                ControlToValidate="txtEmail"
                ErrorMessage="Please enter your email."
                ForeColor="Red" />

            <asp:Label runat="server" Text="Phone Number" />
            <asp:TextBox ID="txtPhone" runat="server"
                CssClass="form-control" MaxLength="20" />

            <asp:Label runat="server" Text="Password (minimum 8 characters)" />
            <asp:TextBox ID="txtPassword" runat="server"
                TextMode="Password" CssClass="form-control" />

            <asp:RequiredFieldValidator runat="server"
                ControlToValidate="txtPassword"
                ErrorMessage="Please enter a password."
                ForeColor="Red" />

            <asp:Label runat="server" Text="Confirm Password" />
            <asp:TextBox ID="txtConfirmPassword" runat="server"
                TextMode="Password" CssClass="form-control" />

            <asp:CompareValidator runat="server"
                ControlToValidate="txtConfirmPassword"
                ControlToCompare="txtPassword"
                ErrorMessage="Passwords do not match."
                ForeColor="Red" />

            <asp:Button ID="btnRegister" runat="server"
                Text="CREATE ACCOUNT"
                CssClass="btn-primary"
                OnClick="btnRegister_Click" />

            <p style="margin-top:20px;">
                Already have an account?
                <a href="Login.aspx">Log in</a>
            </p>
        </div>

    </div>
</section>

</asp:Content>
