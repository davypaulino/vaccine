using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace vaccine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataForDose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "dose",
                columns: new[] { "id", "applied_at", "created_at", "created_by", "dose_type", "updated_at", "updated_by", "vaccination_id" },
                values: new object[,]
                {
                    { new Guid("10df3852-b344-40d0-acb3-b88952419bef"), new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), (short)1, null, null, new Guid("501239da-1ccb-4540-bda4-2ccb4a89db9f") },
                    { new Guid("306b5c29-77b7-4d28-b5df-08007d88a8d6"), new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), (short)1, null, null, new Guid("186ce810-ba7e-4ec1-84bd-094003988c3b") },
                    { new Guid("8cc63d6b-ca5d-4136-8870-26dd0afd0ca0"), new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), (short)2, null, null, new Guid("186ce810-ba7e-4ec1-84bd-094003988c3b") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "dose",
                keyColumn: "id",
                keyValue: new Guid("10df3852-b344-40d0-acb3-b88952419bef"));

            migrationBuilder.DeleteData(
                table: "dose",
                keyColumn: "id",
                keyValue: new Guid("306b5c29-77b7-4d28-b5df-08007d88a8d6"));

            migrationBuilder.DeleteData(
                table: "dose",
                keyColumn: "id",
                keyValue: new Guid("8cc63d6b-ca5d-4136-8870-26dd0afd0ca0"));
        }
    }
}
