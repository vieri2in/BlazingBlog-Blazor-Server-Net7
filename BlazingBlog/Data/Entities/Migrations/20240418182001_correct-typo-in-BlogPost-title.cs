using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazingBlog.Migrations
{
    /// <inheritdoc />
    public partial class correcttypoinBlogPosttitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Titl",
                table: "BlogPosts",
                newName: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "BlogPosts",
                newName: "Titl");
        }
    }
}
