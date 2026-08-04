using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusSupport.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnforceOneMembershipPerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantMembership_UserId",
                table: "TenantMembership");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMembership_UserId",
                table: "TenantMembership",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantMembership_UserId",
                table: "TenantMembership");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMembership_UserId",
                table: "TenantMembership",
                column: "UserId");
        }
    }
}
