USE [Movie_Watchlist]
GO

-- 1. TvShows Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TvShows]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TvShows](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [TmdbId] [int] NOT NULL,
    [Title] [nvarchar](200) NOT NULL DEFAULT ('Unknown'),
    [Description] [nvarchar](1000) NULL DEFAULT (''),
    [PosterPath] [nvarchar](500) NULL,
    [Rating] [float] NULL DEFAULT ((0)),
    [ReleaseYear] [int] NULL DEFAULT ((0)),
    [GenreId] [int] NULL,
 CONSTRAINT [PK_TvShows] PRIMARY KEY CLUSTERED ([Id] ASC),
 CONSTRAINT [UQ_TvShows_TmdbId] UNIQUE NONCLUSTERED ([TmdbId] ASC)
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TvShows_Genre]') AND parent_object_id = OBJECT_ID(N'[dbo].[TvShows]'))
ALTER TABLE [dbo].[TvShows]  WITH CHECK ADD  CONSTRAINT [FK_TvShows_Genre] FOREIGN KEY([GenreId])
REFERENCES [dbo].[Genre] ([Id])
GO

-- 2. TvShowWatchlistItem Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TvShowWatchlistItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TvShowWatchlistItem](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [nvarchar](450) NOT NULL,
    [TvShowId] [int] NOT NULL,
    [DateAdded] [datetime2](7) NOT NULL DEFAULT (sysdatetime()),
    [IsWatched] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_TvShowWatchlistItem] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TvShowWatchlistItem_TvShows]') AND parent_object_id = OBJECT_ID(N'[dbo].[TvShowWatchlistItem]'))
ALTER TABLE [dbo].[TvShowWatchlistItem]  WITH CHECK ADD  CONSTRAINT [FK_TvShowWatchlistItem_TvShows] FOREIGN KEY([TvShowId])
REFERENCES [dbo].[TvShows] ([Id])
ON DELETE CASCADE
GO

-- 3. TvShowType
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'TvShowType' AND ss.name = N'dbo')
CREATE TYPE [dbo].[TvShowType] AS TABLE(
    [TmdbId] [int] NOT NULL,
    [Title] [nvarchar](200) NULL,
    [Description] [nvarchar](1000) NULL,
    [PosterPath] [nvarchar](500) NULL,
    [Rating] [float] NULL,
    [ReleaseYear] [int] NULL,
    [GenreId] [int] NULL
)
GO

-- 4. sp_SyncTmdbTvShows
CREATE OR ALTER PROCEDURE [dbo].[sp_SyncTmdbTvShows]
    @TvShows [dbo].[TvShowType] READONLY
AS
BEGIN
    SET NOCOUNT ON;
    MERGE [dbo].[TvShows] AS target
    USING @TvShows AS source
    ON (target.TmdbId = source.TmdbId)
    WHEN MATCHED THEN
        UPDATE SET
            target.Title = source.Title,
            target.Description = source.Description,
            target.PosterPath = source.PosterPath,
            target.Rating = source.Rating,
            target.ReleaseYear = source.ReleaseYear,
            target.GenreId = source.GenreId
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (TmdbId, Title, Description, PosterPath, Rating, ReleaseYear, GenreId)
        VALUES (source.TmdbId, source.Title, source.Description, source.PosterPath, source.Rating, source.ReleaseYear, source.GenreId);
END
GO

-- 5. sp_GetTvShowsForUser
CREATE OR ALTER PROCEDURE [dbo].[sp_GetTvShowsForUser]
    @UserId NVARCHAR(450),
    @SearchTerm NVARCHAR(200) = '',
    @GenreId INT = 0,
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @TotalCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @SearchTerm = TRIM(@SearchTerm);

    SELECT 
        t.[Id],
        t.[Title],
        t.[TmdbId],
        t.[Description],
        t.[PosterPath],
        t.[ReleaseYear],
        t.[GenreId],
        t.Rating
    INTO #FilteredTvShows
    FROM [dbo].[TvShows] t
    WHERE 
        (@GenreId = 0 OR t.[GenreId] = @GenreId)
        AND (@SearchTerm = '' OR t.[Title] LIKE '%' + @SearchTerm + '%');

    SELECT @TotalCount = COUNT(*)
    FROM #FilteredTvShows;

    SELECT 
        ft.[Id],
        ft.[Title],
        ft.[TmdbId],
        ft.[Description],
        ft.[PosterPath],
        ft.[ReleaseYear],
        ft.[GenreId],
        ft.Rating,

        CAST(
            CASE WHEN w.[Id] IS NOT NULL 
            THEN 1 ELSE 0 END
        AS BIT) AS [IsInWatchlist],

        g.[Name] AS [GenreName]

    FROM #FilteredTvShows ft

    LEFT JOIN [dbo].[Genre] g
        ON ft.[GenreId] = g.[Id]

    LEFT JOIN [dbo].[TvShowWatchlistItem] w
        ON ft.[Id] = w.[TvShowId]
        AND w.[UserId] = @UserId

    ORDER BY ft.ReleaseYear DESC

    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- 6. sp_GetAllTvShowGenres
CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllTvShowGenres]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT g.[Id], g.[Name]
    FROM [dbo].[Genre] g
    INNER JOIN [dbo].[TvShows] t ON g.[Id] = t.[GenreId]
    ORDER BY g.[Name];
END
GO

-- 7. sp_GetTvShowDetails
CREATE OR ALTER PROCEDURE [dbo].[sp_GetTvShowDetails]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        t.[Id],
        t.[TmdbId],
        t.[Title],
        t.[Description],
        t.[PosterPath],
        t.[Rating],
        t.[ReleaseYear],
        t.[GenreId],
        g.[Name] AS [GenreName]
    FROM [dbo].[TvShows] t
    LEFT JOIN [dbo].[Genre] g ON t.[GenreId] = g.[Id]
    WHERE t.[Id] = @Id;
END
GO

-- 8. sp_GetTvShowByTmdbId
CREATE OR ALTER PROCEDURE [dbo].[sp_GetTvShowByTmdbId]
    @TmdbId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        t.[Id],
        t.[TmdbId],
        t.[Title],
        t.[Description],
        t.[PosterPath],
        t.[Rating],
        t.[ReleaseYear],
        t.[GenreId],
        g.[Name] AS [GenreName]
    FROM [dbo].[TvShows] t
    LEFT JOIN [dbo].[Genre] g ON t.[GenreId] = g.[Id]
    WHERE t.[TmdbId] = @TmdbId;
END
GO

-- 9. sp_AddTvShowToWatchlist
CREATE OR ALTER PROCEDURE [dbo].[sp_AddTvShowToWatchlist]
    @TvShowId INT,
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM [dbo].[TvShowWatchlistItem] WHERE [UserId] = @UserId AND [TvShowId] = @TvShowId)
    BEGIN
        INSERT INTO [dbo].[TvShowWatchlistItem] ([UserId], [TvShowId], [DateAdded], [IsWatched])
        VALUES (@UserId, @TvShowId, SYSDATETIME(), 0);
    END
END
GO

-- 10. sp_RemoveTvShowFromWatchlist
CREATE OR ALTER PROCEDURE [dbo].[sp_RemoveTvShowFromWatchlist]
    @TvShowId INT,
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[TvShowWatchlistItem]
    WHERE [UserId] = @UserId AND [TvShowId] = @TvShowId;
END
GO

-- 11. sp_ToggleTvShowWatchedStatus
CREATE OR ALTER PROCEDURE [dbo].[sp_ToggleTvShowWatchedStatus]
    @TvShowId INT,
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[TvShowWatchlistItem]
    SET [IsWatched] = CASE WHEN [IsWatched] = 1 THEN 0 ELSE 1 END
    WHERE [UserId] = @UserId AND [TvShowId] = @TvShowId;
END
GO

-- 12. sp_GetUserTvShowWatchlist
CREATE OR ALTER PROCEDURE [dbo].[sp_GetUserTvShowWatchlist]
    @UserId NVARCHAR(450),
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @TotalCount INT OUTPUT,
    @WatchedCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        w.[Id] AS WatchlistId,
        w.[UserId],
        w.[TvShowId],
        w.[DateAdded],
        w.[IsWatched],
        t.[TmdbId],
        t.[Title],
        t.[Description],
        t.[PosterPath],
        t.[Rating],
        t.[ReleaseYear],
        t.[GenreId]
    INTO #FilteredWatchlist
    FROM [dbo].[TvShowWatchlistItem] w
    INNER JOIN [dbo].[TvShows] t ON w.[TvShowId] = t.[Id]
    WHERE w.[UserId] = @UserId;

    SELECT @TotalCount = COUNT(*) FROM #FilteredWatchlist;
    SELECT @WatchedCount = SUM(CAST([IsWatched] AS INT)) FROM #FilteredWatchlist;

    SELECT 
        fw.WatchlistId AS Id,
        fw.UserId,
        fw.TvShowId,
        fw.DateAdded,
        fw.IsWatched,
        fw.TmdbId,
        fw.Title,
        fw.Description,
        fw.PosterPath,
        fw.Rating,
        fw.ReleaseYear,
        fw.GenreId,
        g.[Name] AS [GenreName]
    FROM #FilteredWatchlist fw
    LEFT JOIN [dbo].[Genre] g ON fw.[GenreId] = g.[Id]
    ORDER BY fw.[DateAdded] DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
