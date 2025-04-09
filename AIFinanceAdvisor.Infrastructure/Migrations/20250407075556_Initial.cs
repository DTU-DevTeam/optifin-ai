using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIFinanceAdvisor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessageHistoryForAssistants",
                columns: table => new
                {
                    IdMessage = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdConverstation = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessageHistoryForAssistants", x => x.IdMessage);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessageHistoryForUsers",
                columns: table => new
                {
                    IdMessage = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdConverstation = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessageHistoryForUsers", x => x.IdMessage);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessageHistoryForAssistants");

            migrationBuilder.DropTable(
                name: "ChatMessageHistoryForUsers");
        }
    }
}
