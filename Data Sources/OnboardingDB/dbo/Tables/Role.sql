CREATE TABLE [dbo].[Role] (
    [RoleId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]   VARCHAR (50) NOT NULL,
    CONSTRAINT [pk_RoleId] PRIMARY KEY CLUSTERED ([RoleId] ASC)
);

