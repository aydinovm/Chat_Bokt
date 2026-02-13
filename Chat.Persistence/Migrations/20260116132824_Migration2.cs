using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "ChatRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClosedByUserId",
                table: "ChatRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "ChatRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResolvedByUserId",
                table: "ChatRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NewToDepartmentId",
                table: "ChatReassignmentHistory",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OldToDepartmentId",
                table: "ChatReassignmentHistory",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "ChatRequests");

            migrationBuilder.DropColumn(
                name: "ClosedByUserId",
                table: "ChatRequests");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "ChatRequests");

            migrationBuilder.DropColumn(
                name: "ResolvedByUserId",
                table: "ChatRequests");

            migrationBuilder.DropColumn(
                name: "NewToDepartmentId",
                table: "ChatReassignmentHistory");

            migrationBuilder.DropColumn(
                name: "OldToDepartmentId",
                table: "ChatReassignmentHistory");
        }
    }
}
