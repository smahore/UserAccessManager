USE [UserManagement]
GO

CREATE TABLE [dbo].[ApplicationAccessRequests](
    [AccessRequestId] [int] IDENTITY(1,1)  NOT NULL,
    [UserName]        [nvarchar](255)       NOT NULL,
    [RoleName]        [nvarchar](200)       NOT NULL,
    [Status]          [nvarchar](50)        NOT NULL,
    [CreatedDate]     [datetime2](0)        NOT NULL,
    [AppRoleId]       [int]                 NOT NULL,
    [Email]           [varchar](255)        NOT NULL,
    CONSTRAINT [PK_ApplicationAccessRequests] PRIMARY KEY CLUSTERED ([AccessRequestId] ASC),
    CONSTRAINT [CK_AAR_Status] CHECK ([Status] IN ('Pending','Approved','Rejected','Cancelled','Fulfilled'))
)
GO

ALTER TABLE [dbo].[ApplicationAccessRequests] ADD CONSTRAINT [DF_AAR_Status]    DEFAULT ('Pending')           FOR [Status]
ALTER TABLE [dbo].[ApplicationAccessRequests] ADD CONSTRAINT [DF_AAR_CreatedDate] DEFAULT (SYSUTCDATETIME())  FOR [CreatedDate]
ALTER TABLE [dbo].[ApplicationAccessRequests] ADD CONSTRAINT [DF_AAR_AppRoleId] DEFAULT ((0))                 FOR [AppRoleId]
GO

ALTER TABLE [dbo].[ApplicationAccessRequests]
    WITH CHECK ADD CONSTRAINT [FK_ApplicationAccessRequests_ApplicationRoles]
    FOREIGN KEY ([AppRoleId]) REFERENCES [dbo].[ApplicationName] ([AppId])
GO
