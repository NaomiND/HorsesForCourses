using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorsesForCourses.Application.Migrations
{
    /// <inheritdoc />
    public partial class pogingweekdaysweg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "weekDays",
                table: "Courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "weekDays",
                table: "Courses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
