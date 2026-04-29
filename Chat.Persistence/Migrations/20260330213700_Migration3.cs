using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Migration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatReassignmentHistory_Users_NewAssignedUserId",
                table: "ChatReassignmentHistory");

            migrationBuilder.AlterColumn<Guid>(
                name: "NewToDepartmentId",
                table: "ChatReassignmentHistory",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "NewAssignedUserId",
                table: "ChatReassignmentHistory",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "ChatReassignmentHistory",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatReassignmentHistory_Users_NewAssignedUserId",
                table: "ChatReassignmentHistory",
                column: "NewAssignedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatReassignmentHistory_Users_NewAssignedUserId",
                table: "ChatReassignmentHistory");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "ChatReassignmentHistory");

            migrationBuilder.AlterColumn<Guid>(
                name: "NewToDepartmentId",
                table: "ChatReassignmentHistory",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "NewAssignedUserId",
                table: "ChatReassignmentHistory",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatReassignmentHistory_Users_NewAssignedUserId",
                table: "ChatReassignmentHistory",
                column: "NewAssignedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
