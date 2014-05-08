CREATE TABLE [dbo].[FileColumn] (
    [FileColumnId]      INT            IDENTITY (1, 1) NOT NULL,
    [FileId]            INT            NOT NULL,
    [Status]            BIT            NOT NULL,
    [EntityName]        VARCHAR (100)  NULL,
    [EntityDescription] VARCHAR (1000) NULL,
    [Name]              VARCHAR (100)  NULL,
    [Description]       VARCHAR (1000) NULL,
    [FileColumnTypeId]  INT            NULL,
    [FileColumnUnitId]  INT            NULL,
    CONSTRAINT [pk_FileColumn] PRIMARY KEY CLUSTERED ([FileColumnId] ASC),
    CONSTRAINT [fk_FileColumn_FileColumnTypes] FOREIGN KEY ([FileColumnTypeId]) REFERENCES [dbo].[FileColumnTypes] ([FileColumnTypeId]),
    CONSTRAINT [fk_FileColumn_FileColumnUnits] FOREIGN KEY ([FileColumnUnitId]) REFERENCES [dbo].[FileColumnUnits] ([FileColumnUnitId]),
    CONSTRAINT [fk_FileColumn_FileId] FOREIGN KEY ([FileId]) REFERENCES [dbo].[File] ([FileId])
);
GO

CREATE NONCLUSTERED INDEX IX_FileColumn_FileId ON FileColumn(FileId DESC)
GO
