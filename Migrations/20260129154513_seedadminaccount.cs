using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTTelecomBE.Migrations
{
    /// <inheritdoc />
    public partial class seedadminaccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash", "Phone", "Role" },
                values: new object[] { 1, new DateTime(2026, 1, 29, 15, 45, 13, 92, DateTimeKind.Utc).AddTicks(983), "admin@fptbinhdinh.com", "Admin FPT Bình Định", "$2a$11$BgCSuZPvr1LuotmqitZ5r.fzaYoU.L.ExtbFTVXUkAJmIuLpW8Z9u", "0901234567", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
