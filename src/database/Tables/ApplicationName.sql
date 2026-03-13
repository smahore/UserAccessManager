USE [UserManagement]
GO

CREATE TABLE [dbo].[ApplicationName](
    [AppId]       [int] IDENTITY(1,1) NOT NULL,
    [AppName]     [varchar](100)      NOT NULL,
    [Description] [varchar](255)      NULL,
    [CreatedAt]   [datetime]          NULL,
    CONSTRAINT [PK_ApplicationName] PRIMARY KEY CLUSTERED ([AppId] ASC),
    CONSTRAINT [UQ_ApplicationName_AppName] UNIQUE NONCLUSTERED ([AppName] ASC)
)
GO

ALTER TABLE [dbo].[ApplicationName] ADD DEFAULT (GETDATE()) FOR [CreatedAt]
GO
