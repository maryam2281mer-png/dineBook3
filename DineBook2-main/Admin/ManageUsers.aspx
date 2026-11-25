<%@ Page Title="Manage Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="DineBook.Admin.ManageUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-section">
        <h1>Manage Users</h1>
        <p>Add, update, view, and delete customer accounts.</p>

        <div class="form-card">
            <h2>Add New User</h2>

            <div class="form-group">
                <label>Full Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Phone</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Password</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
            </div>

            <asp:Button ID="btnAdd" runat="server" Text="Add User"
                CssClass="btn-primary" OnClick="btnAdd_Click" />

            <asp:Label ID="lblMessage" runat="server" />
        </div>

        <h2>All Users</h2>

        <asp:GridView ID="gvUsers" runat="server"
            AutoGenerateColumns="False"
            DataKeyNames="UserID"
            CssClass="reservations-table"
            GridLines="None"
            OnRowEditing="gvUsers_RowEditing"
            OnRowCancelingEdit="gvUsers_RowCancelingEdit"
            OnRowUpdating="gvUsers_RowUpdating"
            OnRowDeleting="gvUsers_RowDeleting"
            EmptyDataText="No users found.">

            <Columns>
                <asp:BoundField DataField="UserID" HeaderText="ID" ReadOnly="True" />
                <asp:BoundField DataField="Name" HeaderText="Name" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="Phone" HeaderText="Phone" />

                <asp:CommandField ShowEditButton="True" EditText="Edit"
                    UpdateText="Save" CancelText="Cancel" />

                <asp:CommandField ShowDeleteButton="True" DeleteText="Delete" />
            </Columns>
        </asp:GridView>

        <br />
        <a href="Dashboard.aspx" class="btn-primary">Back to Dashboard</a>
    </section>
</asp:Content>
