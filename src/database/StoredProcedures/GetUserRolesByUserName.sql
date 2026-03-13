USE [UserManagement]
GO

CREATE PROCEDURE [dbo].[GetUserRolesByUserName]
(
    @UserName    NVARCHAR(200),
    @AllowedRoles NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AllowedRoles AS
    (
        SELECT LTRIM(RTRIM(value)) AS RoleName
        FROM STRING_SPLIT(@AllowedRoles, ',')
    )
    SELECT
        u.UserId,
        u.UserName,
        u.Email,
        ar.AppName AS RoleName
    FROM Users u
    INNER JOIN UserRoles ur ON u.UserId = ur.UserId
    INNER JOIN ApplicationName ar ON ur.AppId = ar.AppId
    INNER JOIN AllowedRoles al ON al.RoleName = ar.AppName
    WHERE
        (LOWER(u.UserName) = LOWER(@UserName) OR LOWER(u.Email) = LOWER(@UserName))
        AND u.IsActive = 1;
END
GO
