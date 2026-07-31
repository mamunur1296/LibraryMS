using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryMS.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddLibrarianTrackingToBorrows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "OutboxMessages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledFor",
                table: "OutboxMessages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IssuedById",
                table: "BorrowRecords",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReturnedById",
                table: "BorrowRecords",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRecords_IssuedById",
                table: "BorrowRecords",
                column: "IssuedById");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRecords_ReturnedById",
                table: "BorrowRecords",
                column: "ReturnedById");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRecords_Users_IssuedById",
                table: "BorrowRecords",
                column: "IssuedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRecords_Users_ReturnedById",
                table: "BorrowRecords",
                column: "ReturnedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRecords_Users_IssuedById",
                table: "BorrowRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRecords_Users_ReturnedById",
                table: "BorrowRecords");

            migrationBuilder.DropIndex(
                name: "IX_BorrowRecords_IssuedById",
                table: "BorrowRecords");

            migrationBuilder.DropIndex(
                name: "IX_BorrowRecords_ReturnedById",
                table: "BorrowRecords");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "ScheduledFor",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "IssuedById",
                table: "BorrowRecords");

            migrationBuilder.DropColumn(
                name: "ReturnedById",
                table: "BorrowRecords");
        }
    }
}
