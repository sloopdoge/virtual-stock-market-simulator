CREATE PROCEDURE [dbo].[sp_Companies_Delete] @Id UNIQUEIDENTIFIER
AS
BEGIN
    DELETE
    FROM [dbo].[Companies]
    WHERE [Id] = @Id
END
