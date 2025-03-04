CREATE PROCEDURE [dbo].[sp_Companies_Update] @Id UNIQUEIDENTIFIER,
                                             @Name NVARCHAR(255),
                                             @Description NVARCHAR(MAX),
                                             @UpdatedAt DATETIME2
AS
BEGIN
    UPDATE [dbo].[Companies]
    SET [Name]        = @Name,
        [Description] = @Description,
        [UpdatedAt]   = @UpdatedAt
    WHERE [Id] = @Id
END
