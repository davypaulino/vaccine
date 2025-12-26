using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace vaccine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class SeedPersonTableAndVaccineTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "birthday",
                table: "person",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "person",
                columns: new[] { "id", "birthday", "created_at", "created_by", "document", "name", "updated_at", "updated_by", "user_id" },
                values: new object[,]
                {
                    { new Guid("322c1393-bd8e-4fb6-b074-b8764a16d317"), new DateTime(1995, 11, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1995, 11, 3, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), "09712058093", "Ana Costa", null, null, null },
                    { new Guid("46bdd486-3de7-4977-af3a-200a7ba02773"), new DateTime(1992, 8, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1992, 8, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), "02443959007", "Maria Oliveira", null, null, null },
                    { new Guid("8ce58e9d-81b3-4aa3-a097-396b46175865"), new DateTime(1988, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1988, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), "75251000049", "Carlos Pereira", null, null, null },
                    { new Guid("8cf44a3d-1700-4167-ab56-66a65c5817ba"), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), "99711606097", "Eduarda Marino", null, null, null },
                    { new Guid("dfc67e0d-3ac4-4b81-8ec4-8ec90da29546"), new DateTime(1985, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1985, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), "06693964001", "João Silva", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "vaccine",
                columns: new[] { "id", "available_types", "created_at", "created_by", "name", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { new Guid("05df7267-b874-42b2-944d-7c04c100d85e"), (short)1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), "Febre Amarela", null, null },
                    { new Guid("45e171d4-e07a-42c9-973d-68d947b86b9f"), (short)3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), "Tríplice Viral (Sarampo, Caxumba, Rubéola)", null, null },
                    { new Guid("63f8f8de-1772-4220-ab07-d00f3748a4c9"), (short)1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), "Influenza (Gripe)", null, null },
                    { new Guid("74c6d6e2-f3ad-4ea8-97ec-6fa900425519"), (short)7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), "Hepatite B", null, null },
                    { new Guid("7def8280-c204-49ce-b04c-e3631a9e414f"), (short)27, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("efbc569d-c8ad-463a-9158-27cdb8d8630a"), "COVID-19", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "person",
                keyColumn: "id",
                keyValue: new Guid("322c1393-bd8e-4fb6-b074-b8764a16d317"));

            migrationBuilder.DeleteData(
                table: "person",
                keyColumn: "id",
                keyValue: new Guid("46bdd486-3de7-4977-af3a-200a7ba02773"));

            migrationBuilder.DeleteData(
                table: "person",
                keyColumn: "id",
                keyValue: new Guid("8ce58e9d-81b3-4aa3-a097-396b46175865"));

            migrationBuilder.DeleteData(
                table: "person",
                keyColumn: "id",
                keyValue: new Guid("8cf44a3d-1700-4167-ab56-66a65c5817ba"));

            migrationBuilder.DeleteData(
                table: "person",
                keyColumn: "id",
                keyValue: new Guid("dfc67e0d-3ac4-4b81-8ec4-8ec90da29546"));

            migrationBuilder.DeleteData(
                table: "vaccine",
                keyColumn: "id",
                keyValue: new Guid("05df7267-b874-42b2-944d-7c04c100d85e"));

            migrationBuilder.DeleteData(
                table: "vaccine",
                keyColumn: "id",
                keyValue: new Guid("45e171d4-e07a-42c9-973d-68d947b86b9f"));

            migrationBuilder.DeleteData(
                table: "vaccine",
                keyColumn: "id",
                keyValue: new Guid("63f8f8de-1772-4220-ab07-d00f3748a4c9"));

            migrationBuilder.DeleteData(
                table: "vaccine",
                keyColumn: "id",
                keyValue: new Guid("74c6d6e2-f3ad-4ea8-97ec-6fa900425519"));

            migrationBuilder.DeleteData(
                table: "vaccine",
                keyColumn: "id",
                keyValue: new Guid("7def8280-c204-49ce-b04c-e3631a9e414f"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "birthday",
                table: "person",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }
    }
}
