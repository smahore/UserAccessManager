USE [UserManagement]
GO

CREATE TABLE [dbo].[StaggingUsers](
    [UserId]   [int] IDENTITY(1,1) NOT NULL,
    [UserName] [nvarchar](150)     NOT NULL,
    [FullName] [nvarchar](200)     NULL,
    [Email]    [nvarchar](300)     NULL,
    [Source]   [varchar](20)       NOT NULL,
    CONSTRAINT [PK_StaggingUsers] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [UQ_StaggingUsers_UserName] UNIQUE NONCLUSTERED ([UserName] ASC)
)
GO
