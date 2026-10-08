/*
    Migration: store the full release date instead of only the year.

    - Movie.ReleaseDate / TvShows.ReleaseDate  -> new DATE column (the real stored value)
    - Movie.ReleaseYear / TvShows.ReleaseYear  -> becomes a computed column = YEAR(ReleaseDate)
      so every existing "list / poster card" procedure that reads ReleaseYear keeps working unchanged.
    - Existing rows are backfilled with 1 January of their old year; the TMDB sync worker
      overwrites them with the real date the next time each item is synced.

    Take a backup before running:
        BACKUP DATABASE [Movie_Watchlist] TO DISK = N'C:\Backup\Movie_Watchlist_before_ReleaseDate.bak';
*/
USE [Movie_Watchlist];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ------------------------------------------------------------------
   1. Tables: add ReleaseDate, backfill, turn ReleaseYear into computed
   ------------------------------------------------------------------ */
DECLARE @tables TABLE (Name SYSNAME);
INSERT INTO @tables VALUES (N'Movie'), (N'TvShows');

DECLARE @t SYSNAME, @sql NVARCHAR(MAX), @df SYSNAME;
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT Name FROM @tables;
OPEN c;
FETCH NEXT FROM c INTO @t;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF COL_LENGTH(N'dbo.' + @t, N'ReleaseDate') IS NULL
    BEGIN
        SET @sql = N'ALTER TABLE [dbo].' + QUOTENAME(@t) + N' ADD [ReleaseDate] DATE NULL;';
        EXEC (@sql);
    END

    IF COLUMNPROPERTY(OBJECT_ID(N'dbo.' + @t), N'ReleaseYear', 'IsComputed') = 0
    BEGIN
        SET @sql = N'UPDATE [dbo].' + QUOTENAME(@t) + N'
                     SET [ReleaseDate] = DATEFROMPARTS([ReleaseYear], 1, 1)
                     WHERE [ReleaseDate] IS NULL AND [ReleaseYear] BETWEEN 1 AND 9999;';
        EXEC (@sql);

        SELECT @df = dc.name
        FROM sys.default_constraints dc
        JOIN sys.columns col ON col.object_id = dc.parent_object_id AND col.column_id = dc.parent_column_id
        WHERE dc.parent_object_id = OBJECT_ID(N'dbo.' + @t) AND col.name = N'ReleaseYear';

        IF @df IS NOT NULL
        BEGIN
            SET @sql = N'ALTER TABLE [dbo].' + QUOTENAME(@t) + N' DROP CONSTRAINT ' + QUOTENAME(@df) + N';';
            EXEC (@sql);
            SET @df = NULL;
        END

        SET @sql = N'ALTER TABLE [dbo].' + QUOTENAME(@t) + N' DROP COLUMN [ReleaseYear];';
        EXEC (@sql);
    END

    IF COL_LENGTH(N'dbo.' + @t, N'ReleaseYear') IS NULL
    BEGIN
        SET @sql = N'ALTER TABLE [dbo].' + QUOTENAME(@t) + N' ADD [ReleaseYear] AS (ISNULL(YEAR([ReleaseDate]), 0));';
        EXEC (@sql);
    END

    FETCH NEXT FROM c INTO @t;
END
CLOSE c;
DEALLOCATE c;
GO

/* ------------------------------------------------------------------
   2. Table types used by the TMDB sync (column order must match the
      DataTable built in MovieRepository / TvShowRepository)
   ------------------------------------------------------------------ */
DROP PROCEDURE IF EXISTS [dbo].[sp_SyncTmdbMovies];
DROP PROCEDURE IF EXISTS [dbo].[sp_SyncTmdbTvShows];
GO
DROP TYPE IF EXISTS [dbo].[MovieType];
DROP TYPE IF EXISTS [dbo].[TvShowType];
GO

CREATE TYPE [dbo].[MovieType] AS TABLE(
    [TmdbId]       INT            NOT NULL,
    [Title]        NVARCHAR(MAX)  NULL,
    [Description]  NVARCHAR(MAX)  NULL,
    [PosterPath]   NVARCHAR(MAX)  NULL,
    [BackdropPath] NVARCHAR(MAX)  NULL,
    [ReleaseDate]  DATE           NULL,
    [GenreId]      INT            NULL,
    [VoteAverage]  FLOAT          NULL
);
GO

CREATE TYPE [dbo].[TvShowType] AS TABLE(
    [TmdbId]       INT            NOT NULL,
    [Title]        NVARCHAR(MAX)  NULL,
    [Description]  NVARCHAR(MAX)  NULL,
    [PosterPath]   NVARCHAR(MAX)  NULL,
    [BackdropPath] NVARCHAR(MAX)  NULL,
    [Rating]       FLOAT          NULL,
    [ReleaseDate]  DATE           NULL,
    [GenreId]      INT            NULL
);
GO

/* ------------------------------------------------------------------
   3. Sync procedures (now write ReleaseDate)
   ------------------------------------------------------------------ */
CREATE OR ALTER PROCEDURE [dbo].[sp_SyncTmdbMovies]
    @Movies [dbo].[MovieType] READONLY
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [dbo].[Movie] AS target
    USING @Movies AS source
    ON (target.[TmdbId] = source.[TmdbId])
    WHEN MATCHED THEN
        UPDATE SET
            target.[Title]        = source.[Title],
            target.[Description]  = source.[Description],
            target.[PosterPath]   = source.[PosterPath],
            target.[BackdropPath] = source.[BackdropPath],
            target.[ReleaseDate]  = source.[ReleaseDate],
            target.[GenreId]      = source.[GenreId],
            target.[VoteAverage]  = source.[VoteAverage]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT ([TmdbId], [Title], [Description], [PosterPath], [BackdropPath], [ReleaseDate], [GenreId], [VoteAverage])
        VALUES (source.[TmdbId], source.[Title], source.[Description], source.[PosterPath], source.[BackdropPath], source.[ReleaseDate], source.[GenreId], source.[VoteAverage]);
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_SyncTmdbTvShows]
    @TvShows [dbo].[TvShowType] READONLY
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [dbo].[TvShows] AS target
    USING @TvShows AS source
    ON (target.[TmdbId] = source.[TmdbId])
    WHEN MATCHED THEN
        UPDATE SET
            target.[Title]        = source.[Title],
            target.[Description]  = source.[Description],
            target.[PosterPath]   = source.[PosterPath],
            target.[BackdropPath] = source.[BackdropPath],
            target.[Rating]       = source.[Rating],
            target.[ReleaseDate]  = source.[ReleaseDate],
            target.[GenreId]      = source.[GenreId]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT ([TmdbId], [Title], [Description], [PosterPath], [BackdropPath], [Rating], [ReleaseDate], [GenreId])
        VALUES (source.[TmdbId], source.[Title], source.[Description], source.[PosterPath], source.[BackdropPath], source.[Rating], source.[ReleaseDate], source.[GenreId]);
END
GO

/* ------------------------------------------------------------------
   4. Details procedures (also return ReleaseDate)
   ------------------------------------------------------------------ */
CREATE OR ALTER PROCEDURE [dbo].[sp_GetMovieDetails]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.*, g.[Name] AS [GenreName]
    FROM [dbo].[Movie] m
    LEFT JOIN [dbo].[Genre] g ON m.[GenreId] = g.[Id]
    WHERE m.[Id] = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_GetMovieByTmdbId]
    @TmdbId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.*, g.[Name] AS [GenreName]
    FROM [dbo].[Movie] m
    LEFT JOIN [dbo].[Genre] g ON m.[GenreId] = g.[Id]
    WHERE m.[TmdbId] = @TmdbId;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_GetTvShowDetails]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.*, g.[Name] AS [GenreName]
    FROM [dbo].[TvShows] t
    LEFT JOIN [dbo].[Genre] g ON t.[GenreId] = g.[Id]
    WHERE t.[Id] = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_GetTvShowByTmdbId]
    @TmdbId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.*, g.[Name] AS [GenreName]
    FROM [dbo].[TvShows] t
    LEFT JOIN [dbo].[Genre] g ON t.[GenreId] = g.[Id]
    WHERE t.[TmdbId] = @TmdbId;
END
GO

/* ------------------------------------------------------------------
   5. Admin procedures (now take @ReleaseDate)
   ------------------------------------------------------------------ */
CREATE OR ALTER PROCEDURE [dbo].[sp_GetMovieById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [dbo].[Movie] WHERE [Id] = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_InsertMovie]
    @Title       NVARCHAR(200),
    @TmdbId      INT,
    @Description NVARCHAR(1000) = NULL,
    @PosterPath  NVARCHAR(MAX)  = NULL,
    @ReleaseDate DATE           = NULL,
    @GenreId     INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[Movie] ([Title], [TmdbId], [Description], [PosterPath], [ReleaseDate], [GenreId])
    VALUES (@Title, @TmdbId, ISNULL(@Description, ''), @PosterPath, @ReleaseDate, @GenreId);
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_UpdateMovie]
    @Id          INT,
    @Title       NVARCHAR(200),
    @TmdbId      INT,
    @Description NVARCHAR(1000) = NULL,
    @PosterPath  NVARCHAR(MAX)  = NULL,
    @ReleaseDate DATE           = NULL,
    @GenreId     INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Movie]
    SET [Title]       = @Title,
        [TmdbId]      = @TmdbId,
        [Description] = ISNULL(@Description, ''),
        [PosterPath]  = @PosterPath,
        [ReleaseDate] = @ReleaseDate,
        [GenreId]     = @GenreId
    WHERE [Id] = @Id;
END
GO

/* ------------------------------------------------------------------
   6. Check: procedures that still mention ReleaseYear.
      Reading ReleaseYear is fine (lists / poster cards). Any listed procedure that
      INSERTs or UPDATEs ReleaseYear must be changed to write ReleaseDate instead.
   ------------------------------------------------------------------ */
SELECT p.name AS ProcedureStillMentioningReleaseYear
FROM sys.procedures p
WHERE OBJECT_DEFINITION(p.object_id) LIKE N'%ReleaseYear%'
ORDER BY p.name;
GO
