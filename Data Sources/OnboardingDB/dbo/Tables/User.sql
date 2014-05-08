CREATE TABLE [dbo].[User] (
    [UserId]           INT         IDENTITY (1, 1) NOT NULL,
    [NameIdentifier]   VARCHAR (256)   NOT NULL,
    [IdentityProvider] VARCHAR (128)   NULL,
    [FirstName]        NVARCHAR (64) NULL,
    [MiddleName]       NVARCHAR (64) NULL,
    [LastName]         NVARCHAR (64) NULL,
    [Organization]     NVARCHAR (128) NULL,
    [EmailId]          NVARCHAR (128) NULL,
    [IsActive]         BIT            NOT NULL DEFAULT 1,
    [CreatedOn]        DATETIME       NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedOn]       DATETIME       NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [pk_UserId] PRIMARY KEY CLUSTERED ([UserId] ASC), 
    CONSTRAINT [AK_User_NameIdentifier] UNIQUE ([NameIdentifier])
);
GO;
CREATE NONCLUSTERED INDEX [IX_User_NameIdentifier]
    ON [dbo].[User]([NameIdentifier] ASC) WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
GO;

CREATE TRIGGER [dbo].[TRG_User_UpdateModifiedOn]
    ON [dbo].[User]
    AFTER UPDATE
    AS
    BEGIN
        SET NoCount ON;
        UPDATE [dbo].[User]
            SET [ModifiedOn] = GETUTCDATE()
            FROM [dbo].[User] u
            INNER JOIN inserted I on u.UserId = I.UserId
    END
GO;