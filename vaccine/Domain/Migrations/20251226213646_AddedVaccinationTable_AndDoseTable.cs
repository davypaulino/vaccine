using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vaccine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedVaccinationTable_AndDoseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vaccination",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccine_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccination", x => x.id);
                    table.ForeignKey(
                        name: "FK_vaccination_person_person_id",
                        column: x => x.person_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vaccination_vaccine_vaccine_id",
                        column: x => x.vaccine_id,
                        principalTable: "vaccine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dose",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccination_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dose_type = table.Column<short>(type: "smallint", nullable: false),
                    applied_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dose", x => x.id);
                    table.ForeignKey(
                        name: "FK_dose_vaccination_vaccination_id",
                        column: x => x.vaccination_id,
                        principalTable: "vaccination",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dose_vaccination_id",
                table: "dose",
                column: "vaccination_id");

            migrationBuilder.CreateIndex(
                name: "IX_vaccination_person_id",
                table: "vaccination",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_vaccination_vaccine_id",
                table: "vaccination",
                column: "vaccine_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dose");

            migrationBuilder.DropTable(
                name: "vaccination");
        }
    }
}
