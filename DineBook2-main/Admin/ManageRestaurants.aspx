
<%@ Page Title="Manage Restaurants" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageRestaurants.aspx.cs" Inherits="DineBook.Admin.ManageRestaurants" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-section">
        <h1>Manage Restaurants</h1>
        <p>Add, update, and manage restaurant information.</p>

        <div class="form-card">
            <h2>Add Restaurant</h2>

            <div class="form-group">
                <label>Restaurant Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Description</label>
                <asp:TextBox ID="txtDescription" runat="server"
                    TextMode="MultiLine" Rows="4" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Reservation Duration (Minutes)</label>
                <asp:TextBox ID="txtDuration" runat="server"
                    TextMode="Number" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Average Price</label>
                <asp:TextBox ID="txtPrice" runat="server"
                    TextMode="Number" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Maximum Guests per Reservation</label>
                <asp:TextBox ID="txtMaxGuests" runat="server"
                    TextMode="Number" Text="8" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Restaurant Image URL</label>
                <asp:TextBox ID="txtPhoto" runat="server" CssClass="form-control" />
            </div>

            <asp:Button ID="btnAdd" runat="server"
                Text="Add Restaurant"
                CssClass="btn-primary"
                OnClick="btnAdd_Click" />

            <asp:Label ID="lblMessage" runat="server" />
        </div>

        <h2>All Restaurants</h2>

        <asp:GridView ID="gvRestaurants" runat="server"
            AutoGenerateColumns="False"
            DataKeyNames="ServiceID"
            CssClass="reservations-table"
            GridLines="None"
            OnRowEditing="gvRestaurants_RowEditing"
            OnRowCancelingEdit="gvRestaurants_RowCancelingEdit"
            OnRowUpdating="gvRestaurants_RowUpdating"
            OnRowDeleting="gvRestaurants_RowDeleting"
            EmptyDataText="No restaurants found.">

            <Columns>
                <asp:BoundField DataField="ServiceID" HeaderText="ID" ReadOnly="True" />
                <asp:BoundField DataField="ServiceName" HeaderText="Restaurant Name" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:BoundField DataField="Duration" HeaderText="Duration" />
                <asp:BoundField DataField="Price" HeaderText="Average Price" />
                <asp:BoundField DataField="MaxGuests" HeaderText="Max Guests" />
                <asp:BoundField DataField="ServicePhoto" HeaderText="Image URL" />

                <asp:CommandField ShowEditButton="True" />
                <asp:CommandField ShowDeleteButton="True" />
            </Columns>
        </asp:GridView>

        <br />
        <a href="Dashboard.aspx" class="btn-primary">Back to Dashboard</a>
    </section>
</asp:Content>
