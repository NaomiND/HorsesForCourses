using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorsesForCourses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTimeslots3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledTimeSlots",
                table: "Courses");

            migrationBuilder.CreateTable(
                name: "ScheduledTimeSlot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Day = table.Column<string>(type: "TEXT", nullable: false),
                    TimeSlot_StartTime = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeSlot_EndTime = table.Column<int>(type: "INTEGER", nullable: false),
                    CourseId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledTimeSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledTimeSlot_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTimeSlot_CourseId",
                table: "ScheduledTimeSlot",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledTimeSlot");

            migrationBuilder.AddColumn<string>(
                name: "ScheduledTimeSlots",
                table: "Courses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
