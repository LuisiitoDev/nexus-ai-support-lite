using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusSupport.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_Rol_CreateAt")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_Tenant_CreateAt"),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_Tenant_UpdateAt")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantMembershipStatus",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantMembershipStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserStatus",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvider",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderType = table.Column<short>(type: "smallint", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CallbackPath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_IdentityProvider_CreateAt"),
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

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ExternalSubjectId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_User_CreateAt"),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_User_UpdateAt")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_UserStatus_Status",
                        column: x => x.Status,
                        principalTable: "UserStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantMembership",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_TenantMembership_CreatedAt"),
                    JoinAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_TenantMembership_JoinAt"),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_TenantMembership_UpdateAt")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantMembership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantMembership_TenantMembershipStatus_Status",
                        column: x => x.Status,
                        principalTable: "TenantMembershipStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantMembership_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantMembership_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MembershipRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantMembershipId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_MembershipRole_CreateAt")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembershipRole_Rol_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MembershipRole_TenantMembership_TenantMembershipId",
                        column: x => x.TenantMembershipId,
                        principalTable: "TenantMembership",
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

            migrationBuilder.CreateIndex(
                name: "IX_MembershipRole_RoleId",
                table: "MembershipRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipRole_TenantMembershipId",
                table: "MembershipRole",
                column: "TenantMembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMembership_Status",
                table: "TenantMembership",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMembership_TenantId",
                table: "TenantMembership",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMembership_UserId",
                table: "TenantMembership",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Issuer_ExternalSubjectId",
                table: "User",
                columns: new[] { "Issuer", "ExternalSubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Status",
                table: "User",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityProvider");

            migrationBuilder.DropTable(
                name: "MembershipRole");

            migrationBuilder.DropTable(
                name: "IdentityProviderType");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "TenantMembership");

            migrationBuilder.DropTable(
                name: "TenantMembershipStatus");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "UserStatus");
        }
    }
}
