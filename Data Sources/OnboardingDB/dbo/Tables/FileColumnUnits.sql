CREATE TABLE [dbo].[FileColumnUnits] (
    [FileColumnUnitId] INT          IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (50) NULL,
    [FileColumnTypeId] INT          NULL,
    [Status]           BIT          NOT NULL,
    CONSTRAINT [pk_FileColumnUnits] PRIMARY KEY CLUSTERED ([FileColumnUnitId] ASC),
    CONSTRAINT [fk_FileColumnUnits_FileColumnTypes] FOREIGN KEY ([FileColumnTypeId]) REFERENCES [dbo].[FileColumnTypes] ([FileColumnTypeId])
);

