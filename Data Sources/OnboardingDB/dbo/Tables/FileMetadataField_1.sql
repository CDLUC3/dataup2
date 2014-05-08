CREATE TABLE [dbo].[FileMetadataField] (
	[FileMetadataFieldId] [int] IDENTITY(1,1) NOT NULL,
	[FileId] [int] NOT NULL,	
	[RepositoryMetadataFieldId] [int] NOT NULL,	
	[MetadataValue] [varchar](200) NULL
    CONSTRAINT [pk_FileMetadataField] PRIMARY KEY CLUSTERED ([FileMetadataFieldId] ASC),
    CONSTRAINT [fk_FileMetadataDetails_File] FOREIGN KEY ([FileId]) REFERENCES [dbo].[File] ([FileId]),
    CONSTRAINT [fk_FileMetadataDetails_RepositoryMetadataField] FOREIGN KEY ([RepositoryMetadataFieldId]) REFERENCES [dbo].[RepositoryMetadataField] ([RepositoryMetadataFieldId])    
);
GO

CREATE NONCLUSTERED INDEX IX_FileMetadataField_FileId ON FileMetadataField(FileId DESC)
GO