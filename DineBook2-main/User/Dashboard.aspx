<%@ Page Title="My Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="DineBook.User.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-section">
        <h1>My Dashboard</h1>

        <h2>Welcome, <asp:Label ID="lblUserName" runat="server" Text="Guest" />!</h2>

        <p>Manage your restaurant reservations and account.</p>

        <div class="dashboard-grid">
            <div class="dashboard-card">
                <h3>Find a Restaurant</h3>
                <p>Explore restaurants and book your next table.</p>
                <a href="../Restaurants.aspx" class="btn-primary">Explore Restaurants</a>
            </div>

            <div class="dashboard-card">
                <h3>My Reservations</h3>
                <p>View your upcoming and previous reservations.</p>
                <a href="MyReservations.aspx" class="btn-primary">View Reservations</a>
            </div>

            <div class="dashboard-card">
                <h3>My Profile</h3>
                <p>View and update your personal information.</p>
                <a href="Profile.aspx" class="btn-primary">Manage Profile</a>
            </div>
        </div>
    </section>
</asp:Content>
