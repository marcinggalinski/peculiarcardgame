using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeculiarCardGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class storingrefreshtokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TokenInfos",
                columns: table => new
                {
                    Token = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ExpirationDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenInfos", x => x.Token);
                    table.ForeignKey(
                        name: "FK_TokenInfos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenInfos_UserId",
                table: "TokenInfos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenInfos");
        }
    }
}
