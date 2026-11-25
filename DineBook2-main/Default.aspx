
<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DineBook.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <section class="hero-section">
        <div class="hero-content">
            <h1>Discover. Reserve. Enjoy.</h1>

            <p>
                Find your favorite restaurants and reserve your table
                with just a few clicks.
            </p>

            <a href="Restaurants.aspx" class="btn-primary">
                Explore Restaurants
            </a>

            <a href="Account/Register.aspx" class="btn-secondary">
                Create Account
            </a>
        </div>
    </section>

    <section class="page-section">
        <h2>Welcome to DineBook</h2>

        <p>
            DineBook makes restaurant reservations simple and convenient.
            Explore different restaurants, choose your preferred date and
            time, and enjoy a smooth dining experience.
        </p>
    </section>

    <section class="page-section">
        <h2>Why Choose DineBook?</h2>

        <div class="dashboard-grid">

            <div class="dashboard-card">
                <h3>Easy Booking</h3>
                <p>Reserve your favorite restaurant in a few simple steps.</p>
            </div>

            <div class="dashboard-card">
                <h3>Flexible Choices</h3>
                <p>Explore restaurants and select a suitable reservation time.</p>
            </div>

            <div class="dashboard-card">
                <h3>Manage Reservations</h3>
                <p>View and manage your upcoming and previous reservations.</p>
            </div>

        </div>
    </section>

</asp:Content>
