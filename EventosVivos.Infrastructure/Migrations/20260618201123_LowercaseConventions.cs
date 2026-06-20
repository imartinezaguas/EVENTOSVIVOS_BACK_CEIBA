using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventosVivos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LowercaseConventions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Events_EventId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Venues",
                table: "Venues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "Venues",
                newName: "venues");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "events");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "bookings");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "venues",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "venues",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "venues",
                newName: "capacity");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "venues",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "VenueId",
                table: "events",
                newName: "venueid");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "events",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "events",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "events",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "events",
                newName: "startdate");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "events",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "MaxCapacity",
                table: "events",
                newName: "maxcapacity");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "events",
                newName: "enddate");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "events",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "events",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Events_VenueId",
                table: "events",
                newName: "ix_events_venueid");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "bookings",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "ReservationCode",
                table: "bookings",
                newName: "reservationcode");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "bookings",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "bookings",
                newName: "eventid");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "bookings",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "CancelledAt",
                table: "bookings",
                newName: "cancelledat");

            migrationBuilder.RenameColumn(
                name: "BuyerName",
                table: "bookings",
                newName: "buyername");

            migrationBuilder.RenameColumn(
                name: "BuyerEmail",
                table: "bookings",
                newName: "buyeremail");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "bookings",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_EventId",
                table: "bookings",
                newName: "ix_bookings_eventid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_venues",
                table: "venues",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_events",
                table: "events",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bookings",
                table: "bookings",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_events_eventid",
                table: "bookings",
                column: "eventid",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_events_venues_venueid",
                table: "events",
                column: "venueid",
                principalTable: "venues",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bookings_events_eventid",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "fk_events_venues_venueid",
                table: "events");

            migrationBuilder.DropPrimaryKey(
                name: "pk_venues",
                table: "venues");

            migrationBuilder.DropPrimaryKey(
                name: "pk_events",
                table: "events");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bookings",
                table: "bookings");

            migrationBuilder.RenameTable(
                name: "venues",
                newName: "Venues");

            migrationBuilder.RenameTable(
                name: "events",
                newName: "Events");

            migrationBuilder.RenameTable(
                name: "bookings",
                newName: "Bookings");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Venues",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "Venues",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "capacity",
                table: "Venues",
                newName: "Capacity");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Venues",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "venueid",
                table: "Events",
                newName: "VenueId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Events",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Events",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Events",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "startdate",
                table: "Events",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Events",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "maxcapacity",
                table: "Events",
                newName: "MaxCapacity");

            migrationBuilder.RenameColumn(
                name: "enddate",
                table: "Events",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Events",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Events",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_events_venueid",
                table: "Events",
                newName: "IX_Events_VenueId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Bookings",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "reservationcode",
                table: "Bookings",
                newName: "ReservationCode");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "Bookings",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "eventid",
                table: "Bookings",
                newName: "EventId");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "Bookings",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "cancelledat",
                table: "Bookings",
                newName: "CancelledAt");

            migrationBuilder.RenameColumn(
                name: "buyername",
                table: "Bookings",
                newName: "BuyerName");

            migrationBuilder.RenameColumn(
                name: "buyeremail",
                table: "Bookings",
                newName: "BuyerEmail");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Bookings",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_bookings_eventid",
                table: "Bookings",
                newName: "IX_Bookings_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Venues",
                table: "Venues",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Events_EventId",
                table: "Bookings",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
