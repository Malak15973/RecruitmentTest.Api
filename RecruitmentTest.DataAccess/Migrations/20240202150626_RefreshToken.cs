using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentTest.DataAccess.Migrations
{
    public partial class RefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ae75acf7-8bc2-4b43-8e06-4498a63e4c33", "AQAAAAEAACcQAAAAEPe+TYIxl+uTGwivp75Nr/cZ253BgCfKVvy12ltgQsKbX2aom2IZegGTDbG8MYS//A==", "de50a31f-ddaf-480e-9bd5-0fce5b66ce05" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fc963754-eb84-4e27-ba48-c6a36cb33e8c", "AQAAAAEAACcQAAAAEPrZzMDeUEYGBp99+VFCMOMahGFvAKdknfbgYlp8HXdKgOidi+si83pGmyEc+i7QeQ==", "0d709545-7bfb-4469-ad5a-fbea0063a3c3" });
        }
    }
}
