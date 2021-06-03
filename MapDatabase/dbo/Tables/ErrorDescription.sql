CREATE TABLE [dbo].[ErrorDescription] (
    [Id]          INT            NOT NULL,
    [Description] NVARCHAR (250) NULL,
    [NullDate]    DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

