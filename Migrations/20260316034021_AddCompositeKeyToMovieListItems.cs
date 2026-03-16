using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace hitchBackend.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeKeyToMovieListItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieListItems",
                table: "MovieListItems");

            migrationBuilder.DropIndex(
                name: "IX_MovieListItems_MovieListId",
                table: "MovieListItems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MovieListItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieListItems",
                table: "MovieListItems",
                columns: new[] { "MovieListId", "MovieId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieListItems",
                table: "MovieListItems");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MovieListItems",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieListItems",
                table: "MovieListItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MovieListItems_MovieListId",
                table: "MovieListItems",
                column: "MovieListId");
        }
    }
}
