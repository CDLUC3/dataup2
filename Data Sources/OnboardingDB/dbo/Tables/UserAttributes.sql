CREATE TABLE [dbo].[UserAttributes] (
    [UserAttributeId] INT           IDENTITY (1, 1) NOT NULL,
    [UserId]          INT           NOT NULL,
    [Key]             VARCHAR (150) NOT NULL,
    [Value]           VARCHAR(500) NULL,
    CONSTRAINT [pk_UserAttributesId] PRIMARY KEY CLUSTERED ([UserAttributeId] ASC),
    CONSTRAINT [fk_UserAttribute_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);
GO;
CREATE NONCLUSTERED INDEX [IX_UserAttributes_UserId]
    ON [dbo].[UserAttributes]([UserId] ASC) WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
GO;
