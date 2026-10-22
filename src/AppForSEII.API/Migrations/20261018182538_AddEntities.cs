using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppForSEII.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefereeAssignedTo_Referee_RefereeId",
                table: "RefereeAssignedTo");

            migrationBuilder.DropTable(
                name: "Referee");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SportId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsRefereeing",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SportId",
                table: "AspNetUsers",
                column: "SportId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Sports_SportId",
                table: "AspNetUsers",
                column: "SportId",
                principalTable: "Sports",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefereeAssignedTo_AspNetUsers_RefereeId",
                table: "RefereeAssignedTo",
                column: "RefereeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Sports_SportId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RefereeAssignedTo_AspNetUsers_RefereeId",
                table: "RefereeAssignedTo");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SportId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SportId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "YearsRefereeing",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Referee",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    YearsRefereeing = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Referee_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Referee_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Referee_SportId",
                table: "Referee",
                column: "SportId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefereeAssignedTo_Referee_RefereeId",
                table: "RefereeAssignedTo",
                column: "RefereeId",
                principalTable: "Referee",
                principalColumn: "Id");
        }
    }
}
