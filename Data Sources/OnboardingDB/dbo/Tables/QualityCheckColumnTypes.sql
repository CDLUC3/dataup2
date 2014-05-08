
CREATE TABLE [dbo].[QualityCheckColumnTypes]
(
	[QualityCheckColumnTypeId]  INT           IDENTITY (0, 1) NOT NULL,
    [Name]                VARCHAR (50)  NULL,
    [Description]               VARCHAR (250) NULL,
	
	CONSTRAINT [pk_QualityCheckColumnTypes] PRIMARY KEY CLUSTERED ([QualityCheckColumnTypeId] ASC),
);
