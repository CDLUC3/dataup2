CREATE TABLE [dbo].[MetadataTypes] (
    [MetadataTypeId] INT          IDENTITY (1, 1) NOT NULL,
    [Name]           VARCHAR (50) NULL,
    [Status]         BIT          NOT NULL,
    CONSTRAINT [pk_MetadataTypeId] PRIMARY KEY CLUSTERED ([MetadataTypeId] ASC)
);

