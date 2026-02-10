using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FPTTelecomBE.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorySystemWithRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpeedDown = table.Column<int>(type: "int", nullable: false),
                    SpeedUp = table.Column<int>(type: "int", nullable: false),
                    PriceMonthly = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PromotionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceBonus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedStaffId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registrations_Users_AssignedStaffId",
                        column: x => x.AssignedStaffId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Registrations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Active", "CreatedAt", "DisplayOrder", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2026, 2, 10, 14, 1, 27, 123, DateTimeKind.Utc).AddTicks(6150), 1, "Internet Gia Đình", "internet-gia-dinh" },
                    { 2, true, new DateTime(2026, 2, 10, 14, 1, 27, 123, DateTimeKind.Utc).AddTicks(6153), 2, "Combo Đa Dịch Vụ", "combo-da-dich-vu" },
                    { 3, true, new DateTime(2026, 2, 10, 14, 1, 27, 123, DateTimeKind.Utc).AddTicks(6155), 3, "WiFi 7 Cao Cấp", "wifi-7-cao-cap" },
                    { 4, true, new DateTime(2026, 2, 10, 14, 1, 27, 123, DateTimeKind.Utc).AddTicks(6156), 4, "Doanh Nghiệp", "doanh-nghiep" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash", "Phone", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 10, 14, 1, 27, 13, DateTimeKind.Utc).AddTicks(9858), "admin@fptbinhdinh.com", "Admin FPT Bình Định", "$2a$11$r/XqDSP7sVWtf9dLxfhzK.CVzLcsHVlm3IPrMOVgQrjWWVdy.9zTS", "0332766193", "Admin" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff@fptbinhdinh.com", "Staff Quy Nhơn", "$2a$11$Ukel9S36Yb4fnazDHsKGr.gaXjKomqTGou4bMdrX.cLnkJls/QVkm", "0902345678", "Staff" }
                });

            migrationBuilder.InsertData(
                table: "Packages",
                columns: new[] { "Id", "Active", "CategoryId", "DeviceBonus", "ImageUrl", "Name", "PriceMonthly", "PromotionText", "SpeedDown", "SpeedUp" },
                values: new object[,]
                {
                    { 1, true, 1, "Modem WiFi 6", null, "Internet Giga", 180000m, "Tặng Modem WiFi 6 + Giảm 50k online + Tặng 1 tháng nếu trả trước 12 tháng", 300, 300 },
                    { 2, true, 2, "Modem WiFi 6 + FPT Play Box", null, "Internet Sky", 190000m, "Tặng Modem WiFi 6 + FPT Play Box (combo) + Giảm 50k", 1000, 300 },
                    { 3, true, 1, "Modem WiFi 6 + Access Point/Mesh", null, "Internet Meta", 305000m, "Symmetric 1Gbps + WiFi 6 + Mesh/AP tùy F1/F2/F3 + Tặng tháng trả trước", 1000, 1000 },
                    { 4, true, 2, "Modem WiFi 6 + FPT Play Box", null, "Combo Thể Thao Sky", 269000m, "Ngoại hạng Anh + FPT Play Box + Modem WiFi 6", 1000, 300 },
                    { 5, true, 3, "Modem WiFi 7 + 1 Mesh WiFi 7", null, "SpeedX2 Pro (WiFi 7)", 1099000m, "XGS-PON + WiFi 7 + Mesh WiFi 7 + FPT Play VIP", 2000, 2000 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CategoryId",
                table: "Packages",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_AssignedStaffId",
                table: "Registrations",
                column: "AssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_PackageId",
                table: "Registrations",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_UserId",
                table: "Registrations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
