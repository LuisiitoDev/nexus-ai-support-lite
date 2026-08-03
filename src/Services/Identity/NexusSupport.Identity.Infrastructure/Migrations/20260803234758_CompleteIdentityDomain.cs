using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusSupport.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompleteIdentityDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityProvider");

            migrationBuilder.DropTable(
                name: "IdentityProviderType");

            migrationBuilder.AddColumn<string>(
                name: "EntraTenantId",
                table: "Tenant",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Rol",
                columns: new[] { "Id", "Code", "CreateAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Requester", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Default role granted to every user on first valid sign-in. Creates and tracks their own incidents.", true, "Requester" },
                    { 2, "Agent", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Handles and resolves incidents assigned to their responsible topics.", true, "Agent" },
                    { 3, "Administrator", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manages organization users, roles, and account status.", true, "Administrator" }
                });

            migrationBuilder.InsertData(
                table: "TenantMembershipStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (short)1, "Enabled" },
                    { (short)2, "Disabled" }
                });

            migrationBuilder.InsertData(
                table: "UserStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (short)1, "Enabled" },
                    { (short)2, "Disabled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_EntraTenantId",
                table: "Tenant",
                column: "EntraTenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenant_EntraTenantId",
                table: "Tenant");

            migrationBuilder.DeleteData(
                table: "Rol",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rol",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rol",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TenantMembershipStatus",
                keyColumn: "Id",
                keyValue: (short)1);

            migrationBuilder.DeleteData(
                table: "TenantMembershipStatus",
                keyColumn: "Id",
                keyValue: (short)2);

            migrationBuilder.DeleteData(
                table: "UserStatus",
                keyColumn: "Id",
                keyValue: (short)1);

            migrationBuilder.DeleteData(
                table: "UserStatus",
                keyColumn: "Id",
                keyValue: (short)2);

            migrationBuilder.DropColumn(
                name: "EntraTenantId",
                table: "Tenant");

            migrationBuilder.CreateTable(
                name: "IdentityProviderType",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProviderType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvider",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()")
                        .Annotation("Relational:DefaultConstraintName", "DF_IdentityProvider_Id"),
                    CallbackPath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_IdentityProvider_CreateAt"),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ProviderType = table.Column<short>(type: "smallint", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_IdentityProvider_UpdateAt")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvider_IdentityProviderType_ProviderType",
                        column: x => x.ProviderType,
                        principalTable: "IdentityProviderType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdentityProvider_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvider_ProviderType",
                table: "IdentityProvider",
                column: "ProviderType");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvider_TenantId",
                table: "IdentityProvider",
                column: "TenantId");
        }
    }
}
