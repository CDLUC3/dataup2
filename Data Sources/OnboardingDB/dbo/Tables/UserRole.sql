CREATE TABLE [dbo].[UserRole] (
    [UserRoleId] INT IDENTITY (1, 1) NOT NULL,
    [UserId]     INT NOT NULL,
    [RoleId]     INT NOT NULL,
    CONSTRAINT [pk_UserRoleId] PRIMARY KEY CLUSTERED ([UserRoleId] ASC),
    CONSTRAINT [fk_UserRole_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleId]),
    CONSTRAINT [fk_UserRole_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);
GO;
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserRole_UserIdRoleId]
    ON [dbo].[UserRole]([UserId] ASC, [RoleId] ASC) WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
GO;
