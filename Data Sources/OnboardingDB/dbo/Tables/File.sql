CREATE TABLE [dbo].[File] (
    [FileId]       INT             IDENTITY (1, 1) NOT NULL,
    [CreatedBy]    INT             NOT NULL,
    [ModifiedBy]   INT             NULL,
    [RepositoryId] INT             NULL,
	[BlobId]	   NVARCHAR (50)   NULL,
    [Title]        NVARCHAR (1000) NOT NULL,
    [Name]         NVARCHAR (1000) NOT NULL,
    [Description]  NVARCHAR (MAX)  NULL,
    [Size]         FLOAT (53)      NOT NULL,
    [Status]       NVARCHAR (200)  NOT NULL,
    [MimeType]     NVARCHAR (200)  NULL,
    [Citation]     NVARCHAR (500)  NULL,
    [Identifier]   NVARCHAR (150)  NULL,
    [CreatedOn]    DATETIME        NULL,
    [ModifiedOn]   DATETIME        NULL,
	[PublishedOn]   DATETIME        NULL,
    [isDeleted]    BIT             NULL,
	[LifelineInHours] SMALLINT     NULL,
    CONSTRAINT [pk_FileId] PRIMARY KEY CLUSTERED ([FileId] ASC),
    CONSTRAINT [fk_File_RepositoryId] FOREIGN KEY ([RepositoryId]) REFERENCES [dbo].[Repository] ([RepositoryId]),
    CONSTRAINT [fk_File_UserCreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [fk_File_UserModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([UserId])
);
GO

CREATE NONCLUSTERED INDEX IX_File_CreatedByStatus ON [File](CreatedBy, [Status])
GO

CREATE NONCLUSTERED INDEX IX_File_RepositoryId ON [File](RepositoryId DESC)
GO
