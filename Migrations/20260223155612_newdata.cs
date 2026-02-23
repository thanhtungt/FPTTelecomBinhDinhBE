using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FPTTelecomBE.Migrations
{
    /// <inheritdoc />
    public partial class newdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SpeedDown = table.Column<int>(type: "integer", nullable: false),
                    SpeedUp = table.Column<int>(type: "integer", nullable: false),
                    PriceMonthly = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PromotionText = table.Column<string>(type: "text", nullable: false),
                    DeviceBonus = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "ChatSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    UserEmail = table.Column<string>(type: "text", nullable: true),
                    UserPhone = table.Column<string>(type: "text", nullable: true),
                    AssignedStaffId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatSessions_Users_AssignedStaffId",
                        column: x => x.AssignedStaffId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobPostings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EmploymentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExperienceLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SalaryMin = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    SalaryMax = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    SalaryCurrency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Requirements = table.Column<string>(type: "text", nullable: true),
                    Benefits = table.Column<string>(type: "text", nullable: true),
                    NumberOfPositions = table.Column<int>(type: "integer", nullable: false),
                    ApplicationDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostings_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    PackageId = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AssignedStaffId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    SenderName = table.Column<string>(type: "text", nullable: false),
                    SenderEmail = table.Column<string>(type: "text", nullable: false),
                    SenderType = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ChatSessionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatSessions_ChatSessionId",
                        column: x => x.ChatSessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobPostingId = table.Column<int>(type: "integer", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CoverLetter = table.Column<string>(type: "text", nullable: true),
                    ResumeUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReviewedByUserId = table.Column<int>(type: "integer", nullable: true),
                    ReviewNote = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplications_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplications_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Active", "CreatedAt", "Description", "DisplayOrder", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2026, 2, 23, 15, 56, 12, 165, DateTimeKind.Utc).AddTicks(3079), "Các gói cước internet dành cho hộ gia đình với tốc độ và giá cả đa dạng.", 1, "Internet Gia Đình", "internet-gia-dinh" },
                    { 2, true, new DateTime(2026, 2, 23, 15, 56, 12, 165, DateTimeKind.Utc).AddTicks(3081), "Các gói combo tích hợp internet với truyền hình, điện thoại và các dịch vụ khác.", 2, "Combo Đa Dịch Vụ", "combo-da-dich-vu" },
                    { 3, true, new DateTime(2026, 2, 23, 15, 56, 12, 165, DateTimeKind.Utc).AddTicks(3083), "Các gói cước sử dụng công nghệ WiFi 7 mới nhất với tốc độ siêu nhanh và ổn định.", 3, "WiFi 7 Cao Cấp", "wifi-7-cao-cap" },
                    { 4, true, new DateTime(2026, 2, 23, 15, 56, 12, 165, DateTimeKind.Utc).AddTicks(3084), "Các gói cước internet và dịch vụ dành riêng cho doanh nghiệp với yêu cầu cao về tốc độ và độ ổn định.", 4, "Doanh Nghiệp", "doanh-nghiep" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash", "Phone", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 23, 15, 56, 12, 56, DateTimeKind.Utc).AddTicks(509), "admin@fptbinhdinh.com", "Admin FPT Bình Định", "$2a$11$ciTGkTYXfvBW1wdvwvuE0OvXngbutTNnBNFTXzQGjkHGApu43XrTm", "0332766193", "Admin" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff@fptbinhdinh.com", "Staff Quy Nhơn", "$2a$11$QncWqGUF9dkj54sWqSEDOe23garS1lYaSJgUnvVT2m2RFAFSlk87m", "0902345678", "Staff" }
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
                name: "IX_ChatMessages_ChatSessionId",
                table: "ChatMessages",
                column: "ChatSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SessionId",
                table: "ChatMessages",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_AssignedStaffId",
                table: "ChatSessions",
                column: "AssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_SessionId",
                table: "ChatSessions",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_UserId",
                table: "ChatSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobPostingId",
                table: "JobApplications",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_ReviewedByUserId",
                table: "JobApplications",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CreatedByUserId",
                table: "JobPostings",
                column: "CreatedByUserId");

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
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "JobApplications");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "ChatSessions");

            migrationBuilder.DropTable(
                name: "JobPostings");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
