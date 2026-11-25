
<%@ Page Title="Manage Availability" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageAvailability.aspx.cs" Inherits="DineBook.Admin.ManageAvailability" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-section">
        <h1>Manage Availability</h1>
        <p>Set available reservation dates and times for each restaurant.</p>

        <div class="form-card">
            <h2>Add Available Time</h2>

            <div class="form-group">
                <label>Restaurant</label>
                <asp:DropDownList ID="ddlRestaurant" runat="server"
                    CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Available Date</label>
                <asp:TextBox ID="txtDate" runat="server"
                    TextMode="Date" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label>Available Time</label>
                <asp:TextBox ID="txtTime" runat="server"
                    TextMode="Time" CssClass="form-control" />
            </div>

            <asp:Button ID="btnAdd" runat="server"
                Text="Add Availability"
                CssClass="btn-primary"
                OnClick="btnAdd_Click" />

            <asp:Label ID="lblMessage" runat="server" />
        </div>

        <h2>Available Reservation Times</h2>

        <asp:GridView ID="gvAvailability" runat="server"
            AutoGenerateColumns="False"
            DataKeyNames="AvailabilityID"
            CssClass="reservations-table"
            GridLines="None"
            OnRowDeleting="gvAvailability_RowDeleting"
            EmptyDataText="No availability records found.">

            <Columns>
                <asp:BoundField DataField="AvailabilityID" HeaderText="ID" />
                <asp:BoundField DataField="ServiceName" HeaderText="Restaurant" />
                <asp:BoundField DataField="AvailableDate" HeaderText="Date"
                    DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="AvailableTime" HeaderText="Time" />

                <asp:CommandField ShowDeleteButton="True" DeleteText="Delete" />
            </Columns>
        </asp:GridView>

        <br />
        <a href="Dashboard.aspx" class="btn-primary">Back to Dashboard</a>
    </section>
</asp:Content>
