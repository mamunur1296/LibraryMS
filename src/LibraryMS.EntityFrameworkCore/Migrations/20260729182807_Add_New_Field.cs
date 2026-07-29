using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryMS.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Add_New_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Books_BookId1",
                table: "BookCopies");

            migrationBuilder.DropIndex(
                name: "IX_BookCopies_BookId1",
                table: "BookCopies");

            migrationBuilder.DropColumn(
                name: "BookId1",
                table: "BookCopies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookId1",
                table: "BookCopies",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_BookId1",
                table: "BookCopies",
                column: "BookId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Books_BookId1",
                table: "BookCopies",
                column: "BookId1",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
