CREATE TABLE [dbo].[Companies]
(
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR(255)    NOT NULL,
    [Description] NVARCHAR(MAX)    NULL,
    [UpdatedAt]   DATETIME2        NOT NULL,
    [CreatedAt]   DATETIME2        NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED ([Id] ASC),
)
