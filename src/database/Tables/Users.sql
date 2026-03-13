USE [UserManagement]
GO

CREATE TABLE [dbo].[Users](
    [UserId]    [int] IDENTITY(1,1)  NOT NULL,
    [UserName]  [nvarchar](150)      NOT NULL,
    [FullName]  [nvarchar](200)      NULL,
    [Email]     [nvarchar](300)      NULL,
    [Source]    [varchar](20)        NOT NULL,
    [IsActive]  [bit]                NOT NULL,
    [CreatedAt] [datetime]           NOT NULL,
    [UpdatedAt] [datetime]           NOT NULL,
    [CreatedBy] [nvarchar](100)      NOT NULL,
    [UpdatedBy] [nvarchar](100)      NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [UQ_Users_UserName] UNIQUE NONCLUSTERED ([UserName] ASC)
)
GO

ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_IsActive]    DEFAULT ((1))          FOR [IsActive]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_CreatedAt]   DEFAULT (GETDATE())    FOR [CreatedAt]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_UpdatedAt]   DEFAULT (GETDATE())    FOR [UpdatedAt]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_CreatedBy]   DEFAULT ('SAP_SYNC')   FOR [CreatedBy]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_UpdatedBy]   DEFAULT ('SAP_SYNC')   FOR [UpdatedBy]
GO
