
CREATE TABLE [dbo].[BaseRepositories] (
	[BaseRepositoryId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NULL
    CONSTRAINT [pk_BaseRepositories] PRIMARY KEY CLUSTERED ([BaseRepositoryId] ASC)
);