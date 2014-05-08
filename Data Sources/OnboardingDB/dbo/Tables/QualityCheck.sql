CREATE TABLE [dbo].[QualityCheck] (
    [QualityCheckId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]           VARCHAR (100)  NULL,
    [Description]    VARCHAR (1000) NULL,
    [IsActive]       BIT            NULL,
	[IsVisibleToAll] BIT            NULL,
	[EnforceOrder]   BIT            NULL,
    [CreatedBy]      INT            NOT NULL,
    [ModifiedBy]     INT            NULL,
    [CreatedOn]      DATETIME       NOT NULL,
    [ModifiedOn]     DATETIME       NULL,
    CONSTRAINT [pk_QualitCheckId] PRIMARY KEY CLUSTERED ([QualityCheckId] ASC),
    CONSTRAINT [fk_QualityCheck_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [fk_QualityCheck_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([UserId])
);

