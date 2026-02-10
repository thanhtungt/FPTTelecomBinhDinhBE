using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTTelecomBE.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionforCategoryPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 10, 16, 16, 56, 77, DateTimeKind.Utc).AddTicks(2661), "Các gói cước internet dành cho hộ gia đình với tốc độ và giá cả đa dạng." });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 10, 16, 16, 56, 77, DateTimeKind.Utc).AddTicks(2663), "Các gói combo tích hợp internet với truyền hình, điện thoại và các dịch vụ khác." });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 10, 16, 16, 56, 77, DateTimeKind.Utc).AddTicks(2665), "Các gói cước sử dụng công nghệ WiFi 7 mới nhất với tốc độ siêu nhanh và ổn định." });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 10, 16, 16, 56, 77, DateTimeKind.Utc).AddTicks(2666), "Các gói cước internet và dịch vụ dành riêng cho doanh nghiệp với yêu cầu cao về tốc độ và độ ổn định." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 10, 16, 16, 55, 966, DateTimeKind.Utc).AddTicks(4768), "$2a$11$fGrJAfv4AvDz3mYq4BI9ZevT/jzZrqwfnChO2pCGrKTIoxfKjEzr." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$7gEKi.s3EVRVjwsfwIWv/exD.yE34dWr8vXgP1ogMn2QdQkMWin5y");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 10, 14, 1, 27, 123, DateTimeKind.Utc).AddTicks(6150));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 10, 14, 1, 27, 123, DateTimeKind.Utc).AddTicks(6153));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 10, 14, 1, 27, 123, DateTimeKind.Utc).AddTicks(6155));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 10, 14, 1, 27, 123, DateTimeKind.Utc).AddTicks(6156));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 10, 14, 1, 27, 13, DateTimeKind.Utc).AddTicks(9858), "$2a$11$r/XqDSP7sVWtf9dLxfhzK.CVzLcsHVlm3IPrMOVgQrjWWVdy.9zTS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$Ukel9S36Yb4fnazDHsKGr.gaXjKomqTGou4bMdrX.cLnkJls/QVkm");
        }
    }
}
