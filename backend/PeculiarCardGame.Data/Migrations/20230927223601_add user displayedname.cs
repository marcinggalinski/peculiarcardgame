using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeculiarCardGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class adduserdisplayedname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayedName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayedName",
                table: "Users");
        }
    }
}
