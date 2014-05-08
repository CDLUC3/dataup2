CREATE TABLE [dbo].[QualityCheckColumnRule] (
    [QualityCheckColumnRuleId]  INT           IDENTITY (1, 1) NOT NULL,
    [QualityCheckId]            INT           NOT NULL,
    [HeaderName]                VARCHAR (250)  NULL,
    [Description]               VARCHAR (250) NULL,
    [IsRequired]                BIT           NULL,
    [ErrorMessage]              VARCHAR (500) NULL,  
	[QualityCheckColumnTypeId]	INT			  NOT NULL,	
	[Order]						INT           NULL,
	[Range]                     VARCHAR (25)  NULL,
    [IsActive]                  BIT           NULL,
    CONSTRAINT [pk_QualityCheckColumnRule] PRIMARY KEY CLUSTERED ([QualityCheckColumnRuleId] ASC),
	CONSTRAINT [fk_QualityCheckColumnRule_QualityCheckType] FOREIGN KEY ([QualityCheckColumnTypeId]) REFERENCES [dbo].[QualityCheckColumnTypes] ([QualityCheckColumnTypeId]),
	CONSTRAINT [fk_QualityCheckColumnRule_QualityCheck] FOREIGN KEY ([QualityCheckId]) REFERENCES [dbo].[QualityCheck] ([QualityCheckId])   
);

