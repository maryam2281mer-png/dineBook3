
<%@ Page Title="Manage Reservations" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageReservations.aspx.cs" Inherits="DineBook.Admin.ManageReservations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-section">
        <h1>Manage Reservations</h1>
        <p>View and manage all customer reservations.</p>

        <asp:Label ID="lblMessage" runat="server" />

        <div class="table-responsive">
            <asp:GridView ID="gvReservations" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="AppointmentID"
                CssClass="reservations-table"
                GridLines="None"
                OnRowEditing="gvReservations_RowEditing"
                OnRowCancelingEdit="gvReservations_RowCancelingEdit"
                OnRowUpdating="gvReservations_RowUpdating"
                OnRowDeleting="gvReservations_RowDeleting"
                EmptyDataText="No reservations found.">

                <Columns>
                    <asp:BoundField DataField="AppointmentID" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="CustomerName" HeaderText="Customer" ReadOnly="True" />
                    <asp:BoundField DataField="RestaurantName" HeaderText="Restaurant" ReadOnly="True" />
                    <asp:BoundField DataField="AppointmentDate" HeaderText="Date"
                        DataFormatString="{0:yyyy-MM-dd}" ReadOnly="True" />
                    <asp:BoundField DataField="AppointmentTime" HeaderText="Time" ReadOnly="True" />
                    <asp:BoundField DataField="NumberOfGuests" HeaderText="Guests" ReadOnly="True" />

                    <asp:BoundField DataField="Status" HeaderText="Status" />

                    <asp:CommandField ShowEditButton="True"
                        UpdateText="Save" CancelText="Cancel" />

                    <asp:CommandField ShowDeleteButton="True" DeleteText="Delete" />
                </Columns>
            </asp:GridView>
        </div>

        <br />
        <a href="Dashboard.aspx" class="btn-primary">Back to Dashboard</a>
    </section>
</asp:Content>
