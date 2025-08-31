using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetCoreCalendar.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixEventLocationFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Ensure Events.LocationId exists and is NULLABLE
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Events','LocationId') IS NULL
BEGIN
    ALTER TABLE dbo.Events ADD [LocationId] INT NULL;
END
ELSE
BEGIN
    ALTER TABLE dbo.Events ALTER COLUMN [LocationId] INT NULL;
END
");

            // 2) Drop any existing FK on LocationId (name may vary)
            migrationBuilder.Sql(@"
DECLARE @fk NVARCHAR(128);
SELECT TOP 1 @fk = fk.[name]
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
JOIN sys.tables t ON fk.parent_object_id = t.object_id
JOIN sys.columns c ON c.object_id = t.object_id AND c.column_id = fkc.parent_column_id
WHERE t.[name] = 'Events' AND c.[name] = 'LocationId';
IF @fk IS NOT NULL
    EXEC('ALTER TABLE dbo.Events DROP CONSTRAINT [' + @fk + ']');
");

            // 3) Ensure index on LocationId
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Events_LocationId' AND object_id = OBJECT_ID('dbo.Events'))
    CREATE INDEX [IX_Events_LocationId] ON dbo.Events([LocationId]);
");

            // 4) Create the correct FK with ON DELETE SET NULL
            migrationBuilder.Sql(@"
ALTER TABLE dbo.Events
ADD CONSTRAINT [FK_Events_Locations_LocationId]
FOREIGN KEY ([LocationId]) REFERENCES dbo.Locations([Id]) ON DELETE SET NULL;
");

            // 5) Drop leftover shadow FK/column LocationId1 if present
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Events','LocationId1') IS NOT NULL
BEGIN
    DECLARE @fk2 NVARCHAR(128);
    SELECT TOP 1 @fk2 = fk.[name]
    FROM sys.foreign_keys fk
    JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
    JOIN sys.tables t ON fk.parent_object_id = t.object_id
    JOIN sys.columns c ON c.object_id = t.object_id AND c.column_id = fkc.parent_column_id
    WHERE t.[name] = 'Events' AND c.[name] = 'LocationId1';

    IF @fk2 IS NOT NULL
        EXEC('ALTER TABLE dbo.Events DROP CONSTRAINT [' + @fk2 + ']');

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Events_LocationId1' AND object_id = OBJECT_ID('dbo.Events'))
        DROP INDEX [IX_Events_LocationId1] ON dbo.Events;

    ALTER TABLE dbo.Events DROP COLUMN [LocationId1];
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Events_Locations_LocationId')
    ALTER TABLE dbo.Events DROP CONSTRAINT [FK_Events_Locations_LocationId];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Events_LocationId' AND object_id = OBJECT_ID('dbo.Events'))
    DROP INDEX [IX_Events_LocationId] ON dbo.Events;
");
        }
    }
}
