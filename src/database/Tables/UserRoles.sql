USE [UserManagement]
GO

CREATE TABLE [dbo].[UserRoles](
    [UserRoleId] [int] IDENTITY(1,1) NOT NULL,
    [UserId]     [int]               NOT NULL,
    [AppId]      [int]               NOT NULL,
    [CreatedAt]  [datetime]          NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserRoleId] ASC)
)
GO

ALTER TABLE [dbo].[UserRoles] ADD DEFAULT (GETDATE()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[UserRoles]
    WITH CHECK ADD FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[UserRoles]
    WITH CHECK ADD CONSTRAINT [FK_UserRoles_ApplicationName] FOREIGN KEY ([AppId])
    REFERENCES [dbo].[ApplicationName] ([AppId])
GO
