CREATE TABLE [dbo].[AuthToken]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NULL, 
    [RepositoryId] INT NULL, 
    [AccessToken] NVARCHAR(MAX) NULL, 
    [RefreshToken] NVARCHAR(MAX) NULL, 
    [TokenExpiresOn] DATETIME NOT NULL, 
    CONSTRAINT [FK_AuthToken_UserId] FOREIGN KEY ([UserId]) REFERENCES [User]([UserId]), 
    CONSTRAINT [FK_AuthToken_RepositoryId] FOREIGN KEY ([RepositoryId]) REFERENCES [Repository]([RepositoryId]) 
)
