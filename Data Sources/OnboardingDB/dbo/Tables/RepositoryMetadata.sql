CREATE TABLE [dbo].[RepositoryMetadata] (
    [RepositoryMetadataId] INT           IDENTITY (1, 1) NOT NULL,
    [CreatedBy]            INT           NOT NULL,
    [ModifiedBy]           INT           NULL,
    [RepositoryId]         INT           NOT NULL,
    [Name]                 VARCHAR (100) NULL,
    [IsActive]             BIT           NULL,
    [CreatedOn]            DATETIME      NULL,
    [ModifiedOn]           DATETIME      NULL,
    CONSTRAINT [pk_RepositoryMetadataId] PRIMARY KEY CLUSTERED ([RepositoryMetadataId] ASC),
    CONSTRAINT [fk_RepositoryMetadata_UserCreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [fk_RepositoryMetadata_UserModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [fk_RepositryMetadata_RepositoryId] FOREIGN KEY ([RepositoryId]) REFERENCES [dbo].[Repository] ([RepositoryId])
);

