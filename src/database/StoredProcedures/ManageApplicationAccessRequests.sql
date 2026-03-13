USE [UserManagement]
GO

CREATE PROCEDURE [dbo].[ManageApplicationAccessRequests]
    @UserName  NVARCHAR(100),
    @Email     NVARCHAR(255),
    @AppRoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ResultTable TABLE (
        Status   NVARCHAR(50)  NOT NULL,
        Message  NVARCHAR(255) NOT NULL,
        AppRoleId INT          NULL,
        RoleName NVARCHAR(100) NULL
    );

    DECLARE @UserId   INT  = NULL;
    DECLARE @IsActive BIT  = NULL;
    DECLARE @RequestedRoleName NVARCHAR(100);

    SELECT @RequestedRoleName = AppName
    FROM [dbo].[ApplicationName]
    WHERE AppId = @AppRoleId;

    IF @RequestedRoleName IS NULL
    BEGIN
        INSERT INTO @ResultTable (Status, Message, AppRoleId)
        VALUES ('InvalidRole', 'This role ID is invalid or no longer available.', @AppRoleId);
        SELECT * FROM @ResultTable;
        RETURN;
    END

    SELECT @UserId = UserId, @IsActive = IsActive
    FROM [dbo].[Users]
    WHERE Email = @Email;

    IF @UserId IS NOT NULL
    BEGIN
        IF @IsActive = 0
        BEGIN
            INSERT INTO @ResultTable (Status, Message, AppRoleId, RoleName)
            VALUES ('UserInactive', 'This user account is inactive; access requests cannot be submitted.', @AppRoleId, @RequestedRoleName);
            SELECT * FROM @ResultTable;
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM [dbo].[UserRoles] WHERE UserId = @UserId AND AppId = @AppRoleId)
        BEGIN
            INSERT INTO @ResultTable (Status, Message, AppRoleId, RoleName)
            VALUES ('RoleAlreadyAssigned', 'You already have access to this application.', @AppRoleId, @RequestedRoleName);
            SELECT * FROM @ResultTable;
            RETURN;
        END
    END

    IF EXISTS (
        SELECT 1 FROM [dbo].[ApplicationAccessRequests]
        WHERE Email = @Email AND AppRoleId = @AppRoleId AND Status = 'Pending'
    )
    BEGIN
        INSERT INTO @ResultTable (Status, Message, AppRoleId, RoleName)
        VALUES ('DuplicateRequest', 'You already submitted an access request for this application. It is currently pending approval.', @AppRoleId, @RequestedRoleName);
        SELECT * FROM @ResultTable;
        RETURN;
    END

    INSERT INTO [dbo].[ApplicationAccessRequests]
        (UserName, RoleName, Status, CreatedDate, Email, AppRoleId)
    VALUES
        (@UserName, @RequestedRoleName, 'Pending', GETDATE(), @Email, @AppRoleId);

    INSERT INTO @ResultTable (Status, Message, AppRoleId, RoleName)
    VALUES ('Success', 'Request submitted successfully.', @AppRoleId, @RequestedRoleName);

    SELECT * FROM @ResultTable;
END
GO
