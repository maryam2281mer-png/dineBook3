
<%@ Page Title="Restaurants" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Restaurants.aspx.cs"
    Inherits="DineBook.Restaurants" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<section class="page-section">
    <div class="container">

        <h1 class="section-title">Discover Our Restaurants</h1>
        <p>Find your favorite restaurant and reserve your table.</p>

        <div class="restaurant-grid">

            <asp:Repeater ID="rptRestaurants" runat="server">
                <ItemTemplate>

                    <div class="card restaurant-card">

                        <asp:Image ID="imgRestaurant" runat="server"
                            ImageUrl='<%# Eval("ServicePhoto") %>'
                            AlternateText="Restaurant"
                            CssClass="restaurant-image"
                            Visible='<%# !string.IsNullOrEmpty(Convert.ToString(Eval("ServicePhoto"))) %>' />

                        <div class="restaurant-placeholder"
                            runat="server"
                            visible='<%# string.IsNullOrEmpty(Convert.ToString(Eval("ServicePhoto"))) %>'>
                            DineBook
                        </div>

                        <h2><%# Eval("ServiceName") %></h2>

                        <p><%# Eval("Description") %></p>

                        <p>
                            Duration: <%# Eval("Duration") %> minutes
                        </p>

                        <p>
                            Price: <%# Eval("Price", "{0:N2}") %>
                        </p>

                        <p>
                            Up to <%# Eval("MaxGuests") %> guests
                        </p>

                        <a class="btn-primary"
                           href='<%# "RestaurantDetails.aspx?id=" + Eval("ServiceID") %>'>
                            View Details &amp; Book
                        </a>

                    </div>

                </ItemTemplate>
            </asp:Repeater>

        </div>

        <asp:Label ID="lblMessage" runat="server" />

    </div>
</section>

</asp:Content>
