using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTTelecomBE.Migrations
{
    /// <inheritdoc />
    public partial class seeddatastaffaccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 30, 14, 36, 40, 165, DateTimeKind.Utc).AddTicks(8535), "$2a$11$Zf.l2r7JwU1dPwKgrNV4I.A9j6jwe9sQmfeo.D58l6Lcb/40gKNeW" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash", "Phone", "Role" },
                values: new object[] { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff@fptbinhdinh.com", "Staff Quy Nhơn", "$2a$11$x65sbf/sxBgA7OSamNcuEuzA6TfrcgdrVDjYriBOciT5LSNZ5fM8K", "0902345678", "Staff" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 30, 14, 34, 17, 822, DateTimeKind.Utc).AddTicks(1009), "$2a$11$brh0vTJrM9rFIXVPoyzqTedMeW2OcnAoeZkiSLB9ewkGO137No5TO" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash", "Phone", "Role" },
                values: new object[] { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff@fptbinhdinh.com", "Staff Quy Nhơn", "$2a$11$Scu6.fIxeb04O25.wgBnCeAtBGAtceWBCfbTdeFYMveyLb2vYj4ha", "0902345678", "Staff" });
        }
    }
}
