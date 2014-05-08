CREATE TABLE [dbo].[FileQualityCheck] (
    [FileQualityCheckId] INT      IDENTITY (1, 1) NOT NULL,
    [QualityCheckId]     INT      NOT NULL,
    [FileId]             INT      NOT NULL,
    [Status]             BIT      NOT NULL,
    [LastRunDateTime]    DATETIME NOT NULL,
    CONSTRAINT [pk_FileQualityCheck] PRIMARY KEY CLUSTERED ([FileQualityCheckId] ASC),
    CONSTRAINT [fk_FileQualityCheck_File] FOREIGN KEY ([FileId]) REFERENCES [dbo].[File] ([FileId]),
    CONSTRAINT [fk_FileQualityCheck_QualityCheck] FOREIGN KEY ([QualityCheckId]) REFERENCES [dbo].[QualityCheck] ([QualityCheckId])
);
GO

CREATE NONCLUSTERED INDEX IX_FileQualityCheck_FileId ON FileQualityCheck(FileId DESC)
GO
