using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorsesForCourses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coaches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Firstname = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Lastname = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Skills = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coaches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cursus = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Startdate = table.Column<DateOnly>(name: "Start date", type: "TEXT", nullable: false),
                    Enddate = table.Column<DateOnly>(name: "End date", type: "TEXT", nullable: false),
                    Skills = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedCoachId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Coaches_AssignedCoachId",
                        column: x => x.AssignedCoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScheduledTimeSlot",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Day = table.Column<string>(type: "TEXT", nullable: false),
                    Starttime = table.Column<TimeOnly>(name: "Start time", type: "TEXT", nullable: false),
                    Endtime = table.Column<TimeOnly>(name: "End time", type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledTimeSlot", x => new { x.CourseId, x.Id });
                    table.ForeignKey(
                        name: "FK_ScheduledTimeSlot_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_Email",
                table: "Coaches",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_AssignedCoachId",
                table: "Courses",
                column: "AssignedCoachId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledTimeSlot");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Coaches");
        }
    }
}
