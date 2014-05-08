CREATE TABLE [dbo].[RepositoryMetadataField] (
    [RepositoryMetadataFieldId] INT           IDENTITY (1, 1) NOT NULL,
    [RepositoryMetadataId]      INT           NOT NULL,
    [MetadataTypeId]            INT           NULL,
    [Name]                      VARCHAR (50)  NULL,
    [Description]               VARCHAR (200) NULL,
    [IsRequired]                BIT           NULL,
    [Range]                     VARCHAR (20)  NULL,
    [Mapping]                   VARCHAR (200) NULL,
    [Order]                     INT           NULL,
    CONSTRAINT [pk_RepositoryMetadataFieldId] PRIMARY KEY CLUSTERED ([RepositoryMetadataFieldId] ASC),
    CONSTRAINT [fk_RepositoryMetadatadetails_Metadata] FOREIGN KEY ([MetadataTypeId]) REFERENCES [dbo].[MetadataTypes] ([MetadataTypeId]),
    CONSTRAINT [fk_RepositoryMetadatadetails_Repository] FOREIGN KEY ([RepositoryMetadataId]) REFERENCES [dbo].[RepositoryMetadata] ([RepositoryMetadataId])
);

