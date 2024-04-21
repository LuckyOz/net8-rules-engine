using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace net8_rules_engine.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "data_engine",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    v_loop = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_data_engine", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "version_engine",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_engine", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "version_engine_detail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    version_engine_id = table.Column<Guid>(type: "uuid", nullable: true),
                    engine_code = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_engine_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_version_engine_detail_version_engine_version_engine_id",
                        column: x => x.version_engine_id,
                        principalTable: "version_engine",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_version_engine_detail_version_engine_id",
                table: "version_engine_detail",
                column: "version_engine_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "data_engine");

            migrationBuilder.DropTable(
                name: "version_engine_detail");

            migrationBuilder.DropTable(
                name: "version_engine");
        }
    }
}
