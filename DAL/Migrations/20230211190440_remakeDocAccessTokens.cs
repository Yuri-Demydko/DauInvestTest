using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class remakeDocAccessTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentAccessToken");

            migrationBuilder.DropIndex(
                name: "IX_SignOperations_DocumentId",
                table: "SignOperations");

            migrationBuilder.AddColumn<string>(
                name: "DocumentAccessToken",
                table: "SignOperations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "ConfirmationCode",
                type: "uuid",
                nullable: true);
            

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationCode_DocumentId",
                table: "ConfirmationCode",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfirmationCode_Documents_DocumentId",
                table: "ConfirmationCode",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfirmationCode_Documents_DocumentId",
                table: "ConfirmationCode");

            migrationBuilder.DropIndex(
                name: "IX_SignOperations_DocumentId",
                table: "SignOperations");

            migrationBuilder.DropIndex(
                name: "IX_ConfirmationCode_DocumentId",
                table: "ConfirmationCode");

            migrationBuilder.DropColumn(
                name: "DocumentAccessToken",
                table: "SignOperations");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "ConfirmationCode");

            migrationBuilder.CreateTable(
                name: "DocumentAccessToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Expired = table.Column<bool>(type: "boolean", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAccessToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAccessToken_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SignOperations_DocumentId",
                table: "SignOperations",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessToken_DocumentId",
                table: "DocumentAccessToken",
                column: "DocumentId");
        }
    }
}
