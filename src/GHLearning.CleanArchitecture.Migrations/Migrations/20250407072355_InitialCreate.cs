using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GHLearning.CleanArchitecture.Migrations.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterDatabase()
				.Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
				name: "users",
				columns: table => new
				{
					id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
						.Annotation("MySql:CharSet", "ascii"),
					email = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8mb4_general_ci")
						.Annotation("MySql:CharSet", "utf8mb4"),
					first_name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8mb4_general_ci")
						.Annotation("MySql:CharSet", "utf8mb4"),
					last_name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8mb4_general_ci")
						.Annotation("MySql:CharSet", "utf8mb4"),
					password_hash = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
						.Annotation("MySql:CharSet", "utf8mb4")
				},
				constraints: table =>
				{
					table.PrimaryKey("PRIMARY", x => x.id);
				})
				.Annotation("MySql:CharSet", "utf8mb4")
				.Annotation("Relational:Collation", "utf8mb4_general_ci");

			migrationBuilder.CreateTable(
				name: "todo_items",
				columns: table => new
				{
					id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
						.Annotation("MySql:CharSet", "ascii"),
					user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
						.Annotation("MySql:CharSet", "ascii"),
					description = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
						.Annotation("MySql:CharSet", "utf8mb4"),
					due_date = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
					labels = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
						.Annotation("MySql:CharSet", "utf8mb4"),
					is_completed = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
					created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false),
					completed_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
					priority = table.Column<int>(type: "int(11)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PRIMARY", x => x.id);
					table.ForeignKey(
						name: "fk_todo_items_users",
						column: x => x.user_id,
						principalTable: "users",
						principalColumn: "id");
				})
				.Annotation("MySql:CharSet", "utf8mb4")
				.Annotation("Relational:Collation", "utf8mb4_general_ci");

			migrationBuilder.CreateIndex(
				name: "fk_todo_items_users",
				table: "todo_items",
				column: "user_id");

			migrationBuilder.CreateIndex(
				name: "idx_users_email",
				table: "users",
				column: "email",
				unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "todo_items");

			migrationBuilder.DropTable(
				name: "users");
		}
	}
}
