using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusSupport.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdDatabaseDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "User",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:DefaultConstraintName", "DF_User_Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TenantMembership",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:DefaultConstraintName", "DF_TenantMembership_Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Tenant",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:DefaultConstraintName", "DF_Tenant_Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "IdentityProvider",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:DefaultConstraintName", "DF_IdentityProvider_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "User",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()")
                .OldAnnotation("Relational:DefaultConstraintName", "DF_User_Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TenantMembership",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()")
                .OldAnnotation("Relational:DefaultConstraintName", "DF_TenantMembership_Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Tenant",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()")
                .OldAnnotation("Relational:DefaultConstraintName", "DF_Tenant_Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "IdentityProvider",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()")
                .OldAnnotation("Relational:DefaultConstraintName", "DF_IdentityProvider_Id");
        }
    }
}
