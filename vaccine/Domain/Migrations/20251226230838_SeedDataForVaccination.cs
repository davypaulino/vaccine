using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace vaccine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataForVaccination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "vaccination",
                columns: new[] { "id", "created_at", "created_by", "person_id", "updated_at", "updated_by", "vaccine_id" },
                values: new object[,]
                {
                    { new Guid("186ce810-ba7e-4ec1-84bd-094003988c3b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), new Guid("8cf44a3d-1700-4167-ab56-66a65c5817ba"), null, null, new Guid("74c6d6e2-f3ad-4ea8-97ec-6fa900425519") },
                    { new Guid("501239da-1ccb-4540-bda4-2ccb4a89db9f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), new Guid("46bdd486-3de7-4977-af3a-200a7ba02773"), null, null, new Guid("7def8280-c204-49ce-b04c-e3631a9e414f") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "vaccination",
                keyColumn: "id",
                keyValue: new Guid("186ce810-ba7e-4ec1-84bd-094003988c3b"));

            migrationBuilder.DeleteData(
                table: "vaccination",
                keyColumn: "id",
                keyValue: new Guid("501239da-1ccb-4540-bda4-2ccb4a89db9f"));
        }
    }
}
