USE [Movie_Watchlist]; 
GO

-- 1. Create User-Defined Table Type for Bulk Movie Imports
IF TYPE_ID(N'MovieType') IS NULL
BEGIN
    CREATE TYPE [dbo].[MovieType] AS TABLE(
        [TmdbId] [int] NOT NULL,
        [Title] [nvarchar](200) NOT NULL,
        [Description] [nvarchar](1000) NULL,
        [PosterPath] [nvarchar](max) NULL,
        [ReleaseYear] [int] NOT NULL,
        [GenreId] [int] NOT NULL
    )
END
GO

-- 2. Stored Procedure for Bulk Syncing Movies
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
            target.[Title] = source.[Title],
            target.[Description] = source.[Description],
            target.[PosterPath] = source.[PosterPath],
            target.[ReleaseYear] = source.[ReleaseYear],
            target.[GenreId] = source.[GenreId]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT ([TmdbId], [Title], [Description], [PosterPath], [ReleaseYear], [GenreId])
        VALUES (source.[TmdbId], source.[Title], source.[Description], source.[PosterPath], source.[ReleaseYear], source.[GenreId]);
END
GO

-- 3. Stored Procedure for Getting Movies for Home Screen (with Search, Filter, and Watchlist Status)
CREATE OR ALTER PROCEDURE [dbo].[sp_GetMoviesForUser]
    @UserId [nvarchar](450),
    @SearchTerm [nvarchar](200) = NULL,
    @GenreId [int] = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Normalize Search Term
    SET @SearchTerm = LTRIM(RTRIM(ISNULL(@SearchTerm, '')));
    
    SELECT 
        m.[Id],
        m.[Title],
        m.[TmdbId],
        m.[Description],
        m.[PosterPath],
        m.[ReleaseYear],
        m.[GenreId],
        CASE WHEN w.[Id] IS NOT NULL THEN 1 ELSE 0 END AS [IsInWatchlist],
        g.[Name] AS [GenreName]
    FROM [dbo].[Movie] m
    INNER JOIN [dbo].[Genre] g ON m.[GenreId] = g.[Id]
    LEFT JOIN [dbo].[WatchlistItem] w ON m.[Id] = w.[MovieId] AND w.[UserId] = @UserId
    WHERE 
        (@GenreId = 0 OR m.[GenreId] = @GenreId)
        AND 
        (@SearchTerm = '' OR m.[Title] LIKE '%' + @SearchTerm + '%');
END
GO

-- 4. Stored Procedure for User Watchlist
CREATE OR ALTER PROCEDURE [dbo].[sp_GetUserWatchlist]
    @UserId [nvarchar](450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        m.[Id] AS [MovieId],
        m.[Title],
        m.[PosterPath],
        m.[ReleaseYear],
        w.[IsWatched]
    FROM [dbo].[Movie] m
    INNER JOIN [dbo].[WatchlistItem] w ON m.[Id] = w.[MovieId]
    WHERE w.[UserId] = @UserId;
END
GO
