
<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="DineBook.User.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-section">
        <h1>My Profile</h1>
        <p>View and update your personal information.</p>

        <div class="form-card">
            <div class="form-group">
                <label>Full Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="100" />
                <asp:RequiredFieldValidator
                    ID="valName" runat="server"
                    ControlToValidate="txtName"
                    ErrorMessage="Name is required."
                    ForeColor="Red" />
            </div>

            <div class="form-group">
                <label>Email Address</label>
                <asp:TextBox ID="txtEmail" runat="server"
                    TextMode="Email" CssClass="form-control" MaxLength="150" />
                <asp:RequiredFieldValidator
                    ID="valEmail" runat="server"
                    ControlToValidate="txtEmail"
                    ErrorMessage="Email is required."
                    ForeColor="Red" />
            </div>

            <div class="form-group">
                <label>Phone Number</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" MaxLength="20" />
            </div>

            <asp:Button ID="btnSave" runat="server"
                Text="Save Changes"
                CssClass="btn-primary"
                OnClick="btnSave_Click" />

            <br /><br />
            <asp:Label ID="lblMessage" runat="server" />
        </div>
    </section>
</asp:Content>
