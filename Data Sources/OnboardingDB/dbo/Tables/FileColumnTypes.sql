CREATE TABLE [dbo].[FileColumnTypes] (
    [FileColumnTypeId] INT          IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (50) NULL,
    [Status]           BIT          NOT NULL,
    CONSTRAINT [pk_FileColumnType] PRIMARY KEY CLUSTERED ([FileColumnTypeId] ASC)
);

