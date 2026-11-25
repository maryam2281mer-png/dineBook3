
<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="DineBook.About" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<section class="page-section">
    <h1>About DineBook</h1>

    <p>
        DineBook is an online restaurant reservation platform
        designed to make dining experiences easier and more convenient.
    </p>

    <h2>Our Mission</h2>
    <p>
        Our mission is to connect customers with restaurants
        through a simple and reliable table reservation system.
    </p>

    <h2>What We Offer</h2>
    <div class="dashboard-grid">
        <div class="dashboard-card">
            <h3>Easy Reservations</h3>
            <p>Book a table by choosing your restaurant, date, time, and number of guests.</p>
        </div>

        <div class="dashboard-card">
            <h3>Restaurant Choices</h3>
            <p>Explore available restaurants and discover new dining experiences.</p>
        </div>

        <div class="dashboard-card">
            <h3>Reservation Management</h3>
            <p>View and manage your upcoming and previous reservations.</p>
        </div>
    </div>
</section>

</asp:Content>
