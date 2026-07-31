using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZahrawyAirFly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modifiedAircraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalSeats",
                table: "Aircrafts");

            migrationBuilder.AlterColumn<string>(
                name: "SeatLayoutJson",
                table: "Aircrafts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "ManufactureDate",
                table: "Aircrafts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Manufacturer",
                table: "Aircrafts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxRangeKm",
                table: "Aircrafts",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManufactureDate",
                table: "Aircrafts");

            migrationBuilder.DropColumn(
                name: "Manufacturer",
                table: "Aircrafts");

            migrationBuilder.DropColumn(
                name: "MaxRangeKm",
                table: "Aircrafts");

            migrationBuilder.AlterColumn<string>(
                name: "SeatLayoutJson",
                table: "Aircrafts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalSeats",
                table: "Aircrafts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
