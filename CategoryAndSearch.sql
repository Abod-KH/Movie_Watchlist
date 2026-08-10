USE Movie_Watchlist;
GO

-- 1. Create the TmdbCategoryMapping table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TmdbCategoryMapping')
BEGIN
    CREATE TABLE TmdbCategoryMapping (
        TmdbId INT NOT NULL,
        MediaType VARCHAR(10) NOT NULL,
        Category VARCHAR(50) NOT NULL,
        [Rank] INT NOT NULL,
        PRIMARY KEY (TmdbId, MediaType, Category)
    );
END
GO

-- 2. Stored Procedure: sp_UpdateCategoryMappings
CREATE OR ALTER PROCEDURE sp_UpdateCategoryMappings
    @Category VARCHAR(50),
    @MediaType VARCHAR(10),
    @MappingsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM TmdbCategoryMapping WHERE Category = @Category AND MediaType = @MediaType;
    INSERT INTO TmdbCategoryMapping (TmdbId, MediaType, Category, [Rank])
    SELECT JSON_VALUE(value, '$.TmdbId'), @MediaType, @Category, JSON_VALUE(value, '$.Rank')
    FROM OPENJSON(@MappingsJson);
END
GO

-- 3. Stored Procedure: sp_GetCategoryItems
CREATE OR ALTER PROCEDURE sp_GetCategoryItems
    @Category VARCHAR(50),
    @MediaType VARCHAR(10),
    @Page INT = 1,
    @PageSize INT = 20,
    @TotalCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    SELECT @TotalCount = COUNT(*)
    FROM TmdbCategoryMapping
    WHERE Category = @Category AND MediaType = @MediaType;

    IF @MediaType = 'movie'
    BEGIN
        SELECT 
            m.Id, m.TmdbId, m.Title, m.PosterPath, m.VoteAverage AS Rating,
            m.ReleaseYear, 'movie' AS MediaType, m.Description, m.GenreId
        FROM TmdbCategoryMapping c
        JOIN Movie m ON c.TmdbId = m.TmdbId
        WHERE c.Category = @Category AND c.MediaType = @MediaType
        ORDER BY c.[Rank] ASC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE IF @MediaType = 'tv'
    BEGIN
        SELECT 
            t.Id, t.TmdbId, t.Title, t.PosterPath, t.Rating,
            t.ReleaseYear, 'tv' AS MediaType, t.Description, t.GenreId
        FROM TmdbCategoryMapping c
        JOIN TvShows t ON c.TmdbId = t.TmdbId
        WHERE c.Category = @Category AND c.MediaType = @MediaType
        ORDER BY c.[Rank] ASC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
END
GO

-- 4. Stored Procedure: sp_SearchMedia
CREATE OR ALTER PROCEDURE sp_SearchMedia
    @SearchTerm NVARCHAR(100),
    @MediaType VARCHAR(10),
    @Page INT = 1,
    @PageSize INT = 20,
    @TotalCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@Page - 1) * @PageSize;
    DECLARE @WildcardSearch NVARCHAR(102) = '%' + @SearchTerm + '%';

    IF @MediaType = 'all'
    BEGIN
        SELECT @TotalCount = (SELECT COUNT(*) FROM Movie WHERE Title LIKE @WildcardSearch OR Description LIKE @WildcardSearch) +
                             (SELECT COUNT(*) FROM TvShows WHERE Title LIKE @WildcardSearch OR Description LIKE @WildcardSearch);

        WITH CombinedResults AS (
            SELECT Id, TmdbId, Title, PosterPath, VoteAverage AS Rating, ReleaseYear, 'movie' AS MediaType, Description, GenreId
            FROM Movie WHERE Title LIKE @WildcardSearch OR Description LIKE @WildcardSearch
            UNION ALL
            SELECT Id, TmdbId, Title, PosterPath, Rating, ReleaseYear, 'tv' AS MediaType, Description, GenreId
            FROM TvShows WHERE Title LIKE @WildcardSearch OR Description LIKE @WildcardSearch
        )
        SELECT * FROM CombinedResults ORDER BY Title ASC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE IF @MediaType = 'movie'
    BEGIN
        SELECT @TotalCount = COUNT(*) FROM Movie WHERE Title LIKE @WildcardSearch OR Description LIKE @WildcardSearch;
        SELECT Id, TmdbId, Title, PosterPath, VoteAverage AS Rating, ReleaseYear, 'movie' AS MediaType, Description, GenreId
        FROM Movie WHERE Title LIKE @WildcardSearch OR Description LIKE @WildcardSearch
        ORDER BY Title ASC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE IF @MediaType = 'tv'
    BEGIN
        SELECT @TotalCount = COUNT(*) FROM TvShows WHERE Title LIKE @WildcardSearch OR Description LIKE @WildcardSearch;
        SELECT Id, TmdbId, Title, PosterPath, Rating, ReleaseYear, 'tv' AS MediaType, Description, GenreId
        FROM TvShows WHERE Title LIKE @WildcardSearch OR Description LIKE @WildcardSearch
        ORDER BY Title ASC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
END
GO
