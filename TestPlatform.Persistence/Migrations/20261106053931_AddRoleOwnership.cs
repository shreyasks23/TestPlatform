using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestPlatform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Tests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Questions",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Users"
                SET "Role" = CASE
                    WHEN "Email" = 'shreyasks@mail.com' THEN 'Admin'
                    WHEN "Email" = 'ada@example.com' THEN 'TestAuthor'
                    ELSE 'TestTaker'
                END;

                UPDATE "Questions"
                SET "CreatedByUserId" = COALESCE(
                    (SELECT "Id" FROM "Users" WHERE "Email" = 'shreyasks@mail.com' LIMIT 1),
                    (SELECT "Id" FROM "Users" ORDER BY "Id" LIMIT 1)
                );

                UPDATE "Tests"
                SET "CreatedByUserId" = COALESCE(
                    (SELECT "Id" FROM "Users" WHERE "Email" = 'shreyasks@mail.com' LIMIT 1),
                    (SELECT "Id" FROM "Users" ORDER BY "Id" LIMIT 1)
                );
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedByUserId",
                table: "Tests",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedByUserId",
                table: "Questions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_CreatedByUserId",
                table: "Tests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CreatedByUserId",
                table: "Questions",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Users_CreatedByUserId",
                table: "Questions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Users_CreatedByUserId",
                table: "Tests",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Users_CreatedByUserId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Users_CreatedByUserId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_CreatedByUserId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CreatedByUserId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Questions");
        }
    }
}
