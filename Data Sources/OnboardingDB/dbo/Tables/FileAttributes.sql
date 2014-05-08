CREATE TABLE [dbo].[FileAttributes] (
    [FileAttributeId] INT           IDENTITY (1, 1) NOT NULL,
    [FileId]          INT           NOT NULL,
    [Key]             VARCHAR (150) NOT NULL,
    [Value]           VARCHAR (150) NOT NULL,
    CONSTRAINT [pk_FileAttributes] PRIMARY KEY CLUSTERED ([FileAttributeId] ASC),
    CONSTRAINT [fk_FileAttributes_File] FOREIGN KEY ([FileId]) REFERENCES [dbo].[File] ([FileId])
);
GO

CREATE NONCLUSTERED INDEX IX_FileAttributes_FileId ON FileAttributes(FileId DESC)
GO

