using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamingApp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class Categories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Categories_GenreId",
                table: "Games");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Categories_GenreId",
                table: "Games",
                column: "GenreId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Categories_GenreId",
                table: "Games");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Categories_GenreId",
                table: "Games",
                column: "GenreId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
