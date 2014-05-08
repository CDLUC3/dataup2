CREATE TABLE [dbo].[Repository] (
    [RepositoryId]              INT            IDENTITY (1, 1) NOT NULL,
	[BaseRepositoryId]			INT			   NULL,
    [CreatedBy]                 INT            NOT NULL,
    [ModifiedBy]                INT            NULL,
       [Name]                      VARCHAR (100)  NOT NULL,
    [IsImpersonating]           BIT            NULL,
    [ImpersonatingUserName]     VARCHAR (100)  NULL,
    [ImpersonatingPassword]     VARCHAR (100)  NULL,
    [UserAgreement]             VARCHAR (2048) NULL,
    [HttpGetUriTemplate]        VARCHAR (250)  NULL,
    [HttpPostUriTemplate]       VARCHAR (250)  NULL,
    [HttpDeleteUriTemplate]     VARCHAR (250)  NULL,
    [HttpIdentifierUriTemplate] VARCHAR (250)  NULL,
    [IsActive]                  BIT            NULL,
    [AllowedFileTypes]          VARCHAR (250)  NULL,
    [CreatedOn]                 DATETIME       NULL,
    [ModifiedOn]                DATETIME       NULL,
	[IsVisibleToAll]            BIT            NULL,
    [AccessToken] NVARCHAR(MAX) NULL, 
    [RefreshToken] NVARCHAR(MAX) NULL, 
    [Status] VARCHAR(10) NULL, 
    [TokenExpiresOn] DATETIME NULL, 
    CONSTRAINT [pk_RepositoryId] PRIMARY KEY CLUSTERED ([RepositoryId] ASC),
    CONSTRAINT [fk_Repository_UserCreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([UserId]),
	CONSTRAINT [fk_Repository_BaseRepository] FOREIGN KEY ([BaseRepositoryId]) REFERENCES [dbo].[BaseRepositories] ([BaseRepositoryId]),
    CONSTRAINT [fk_Repository_UserModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([UserId])


);

