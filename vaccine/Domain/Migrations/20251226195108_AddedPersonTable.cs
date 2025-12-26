using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vaccine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedPersonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "person_id",
                table: "user");

            migrationBuilder.CreateTable(
                name: "person",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    document = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    birthday = table.Column<DateTime>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person", x => x.id);
                    table.ForeignKey(
                        name: "FK_person_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_document",
                table: "person",
                column: "document",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_user_id",
                table: "person",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "person");

            migrationBuilder.DropIndex(
                name: "IX_user_email",
                table: "user");

            migrationBuilder.AddColumn<Guid>(
                name: "person_id",
                table: "user",
                type: "uuid",
                nullable: true);
        }
    }
}
