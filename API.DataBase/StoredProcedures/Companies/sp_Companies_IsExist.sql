CREATE PROCEDURE [dbo].[sp_Companies_IsExist] @Id UNIQUEIDENTIFIER
AS
BEGIN
    IF EXISTS (SELECT 1 FROM [dbo].[Companies] C WHERE C.[Id] = @Id)
        SELECT 1
    ELSE
        SELECT 0
END
