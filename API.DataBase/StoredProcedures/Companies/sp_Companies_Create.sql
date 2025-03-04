CREATE PROCEDURE [dbo].[sp_Companies_Create] @Id UNIQUEIDENTIFIER,
                                             @Name NVARCHAR(255),
                                             @Description NVARCHAR(MAX),
                                             @UpdatedAt DATETIME2,
                                             @CreatedAt DATETIME2
AS
BEGIN
    INSERT INTO [dbo].[Companies] ([Id], [Name], [Description], [UpdatedAt], [CreatedAt])
    VALUES (@Id, @Name, @Description, @UpdatedAt, @CreatedAt)
END
