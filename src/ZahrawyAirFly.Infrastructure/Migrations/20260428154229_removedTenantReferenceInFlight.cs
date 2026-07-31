using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZahrawyAirFly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removedTenantReferenceInFlight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_AspNetUsers_TenantId",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_AspNetUsers_TenantId1",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_TenantId_DepartureUtc",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_TenantId_Status",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_TenantId1",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "TenantId1",
                table: "Flights");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId1",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flights_TenantId_DepartureUtc",
                table: "Flights",
                columns: new[] { "TenantId", "DepartureUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Flights_TenantId_Status",
                table: "Flights",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Flights_TenantId1",
                table: "Flights",
                column: "TenantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_AspNetUsers_TenantId",
                table: "Flights",
                column: "TenantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_AspNetUsers_TenantId1",
                table: "Flights",
                column: "TenantId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
