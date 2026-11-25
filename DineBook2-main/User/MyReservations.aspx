<%@ Page Title="My Reservations" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyReservations.aspx.cs" Inherits="DineBook.User.MyReservations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-section">
        <h1>My Reservations</h1>
        <p>View your upcoming and previous restaurant reservations.</p>

        <asp:Label ID="lblMessage" runat="server" />

        <div class="table-responsive">
            <asp:GridView ID="gvReservations" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="AppointmentID"
                CssClass="reservations-table"
                EmptyDataText="You have no reservations yet."
                OnRowCommand="gvReservations_RowCommand"
                GridLines="None">
                <Columns>
                    <asp:BoundField DataField="ServiceName" HeaderText="Restaurant" />
                    <asp:BoundField DataField="AppointmentDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="AppointmentTime" HeaderText="Time" />
                    <asp:BoundField DataField="NumberOfGuests" HeaderText="Guests" />
                    <asp:BoundField DataField="SpecialRequest" HeaderText="Special Request" />
                    <asp:BoundField DataField="Status" HeaderText="Status" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkCancel" runat="server"
                                Text="Cancel"
                                CommandName="CancelReservation"
                                CommandArgument='<%# Eval("AppointmentID") %>'
                                OnClientClick="return confirm('Cancel this reservation?');"
                                Visible='<%# CanCancel(Eval("Status"), Eval("AppointmentDate"), Eval("AppointmentTime")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>
</asp:Content>
