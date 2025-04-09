using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIFinanceAdvisor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dataVersion20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatHistoryForUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TopicMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdConversation = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistoryForUsers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatHistoryForUsers");
        }
    }
}
