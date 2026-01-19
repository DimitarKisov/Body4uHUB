using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Body4uHUB.Content.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangedContentEntitiesTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop ALL foreign keys
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Comments_Articles_ArticleId')
                    ALTER TABLE Comments DROP CONSTRAINT FK_Comments_Articles_ArticleId;
                
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Bookmarks_Articles_ArticleId')
                    ALTER TABLE Bookmarks DROP CONSTRAINT FK_Bookmarks_Articles_ArticleId;
                
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Comments_Comments_ParentCommentId')
                    ALTER TABLE Comments DROP CONSTRAINT FK_Comments_Comments_ParentCommentId;
            ");

            // 2. Drop ALL indexes that depend on columns we're changing
            migrationBuilder.Sql(@"
                -- Comments indexes
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Comments_ParentCommentId' AND object_id = OBJECT_ID('Comments'))
                    DROP INDEX IX_Comments_ParentCommentId ON Comments;
                
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Comments_ArticleId' AND object_id = OBJECT_ID('Comments'))
                    DROP INDEX IX_Comments_ArticleId ON Comments;
                
                -- Bookmarks indexes (including composite index!)
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookmarks_ArticleId' AND object_id = OBJECT_ID('Bookmarks'))
                    DROP INDEX IX_Bookmarks_ArticleId ON Bookmarks;
                
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookmarks_UserId_ArticleId' AND object_id = OBJECT_ID('Bookmarks'))
                    DROP INDEX IX_Bookmarks_UserId_ArticleId ON Bookmarks;
            ");

            // 3. ForumTopics changes
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ForumTopics",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            // 4. ForumPosts changes
            migrationBuilder.AlterColumn<Guid>(
                name: "ForumTopicId",
                table: "ForumPosts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ForumPosts",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // 5. Comments - Content change
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            // 6. Comments - ParentCommentId
            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentCommentId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true);

            // 7. Comments - ArticleId
            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "ArticleId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);

            // 8. Comments - Id
            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            // 9. Bookmarks - ArticleId
            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Bookmarks");

            migrationBuilder.AddColumn<Guid>(
                name: "ArticleId",
                table: "Bookmarks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<int>(
                name: "ArticleNumber",
                table: "Bookmarks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // 10. Articles - Id and ArticleNumber
            migrationBuilder.DropPrimaryKey(
                name: "PK_Articles",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Articles");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Articles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<int>(
                name: "ArticleNumber",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Articles",
                table: "Articles",
                column: "Id");

            // 11. Recreate indexes
            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ArticleId",
                table: "Comments",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_ArticleId",
                table: "Bookmarks",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_UserId_ArticleId",
                table: "Bookmarks",
                columns: new[] { "UserId", "ArticleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_ArticleNumber",
                table: "Bookmarks",
                column: "ArticleNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleNumber",
                table: "Articles",
                column: "ArticleNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Id",
                table: "Articles",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Comments_ParentCommentId' AND object_id = OBJECT_ID('Comments'))
                    DROP INDEX IX_Comments_ParentCommentId ON Comments;
                
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Comments_ArticleId' AND object_id = OBJECT_ID('Comments'))
                    DROP INDEX IX_Comments_ArticleId ON Comments;
                
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookmarks_ArticleId' AND object_id = OBJECT_ID('Bookmarks'))
                    DROP INDEX IX_Bookmarks_ArticleId ON Bookmarks;
                
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookmarks_UserId_ArticleId' AND object_id = OBJECT_ID('Bookmarks'))
                    DROP INDEX IX_Bookmarks_UserId_ArticleId ON Bookmarks;
                
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookmarks_ArticleNumber' AND object_id = OBJECT_ID('Bookmarks'))
                    DROP INDEX IX_Bookmarks_ArticleNumber ON Bookmarks;
                
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Articles_ArticleNumber' AND object_id = OBJECT_ID('Articles'))
                    DROP INDEX IX_Articles_ArticleNumber ON Articles;
                
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Articles_Id' AND object_id = OBJECT_ID('Articles'))
                    DROP INDEX IX_Articles_Id ON Articles;
            ");

            migrationBuilder.DropColumn(
                name: "ArticleNumber",
                table: "Bookmarks");

            migrationBuilder.DropColumn(
                name: "ArticleNumber",
                table: "Articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Articles",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Articles");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Articles",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Articles",
                table: "Articles",
                column: "Id");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Bookmarks");

            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "Bookmarks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Comments",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "ParentCommentId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ForumTopics",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "ForumTopicId",
                table: "ForumPosts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ForumPosts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Comments_ParentCommentId' AND object_id = OBJECT_ID('Comments'))
                    CREATE INDEX IX_Comments_ParentCommentId ON Comments(ParentCommentId);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Comments_ArticleId' AND object_id = OBJECT_ID('Comments'))
                    CREATE INDEX IX_Comments_ArticleId ON Comments(ArticleId);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookmarks_ArticleId' AND object_id = OBJECT_ID('Bookmarks'))
                    CREATE INDEX IX_Bookmarks_ArticleId ON Bookmarks(ArticleId);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookmarks_UserId_ArticleId' AND object_id = OBJECT_ID('Bookmarks'))
                    CREATE UNIQUE INDEX IX_Bookmarks_UserId_ArticleId ON Bookmarks(UserId, ArticleId);
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Articles_ArticleId",
                table: "Bookmarks",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }
    }
}