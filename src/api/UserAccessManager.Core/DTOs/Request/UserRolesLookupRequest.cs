namespace UserAccessManager.Core.DTOs.Request;

public class UserRolesLookupRequest
{
    public string UserName { get; set; } = string.Empty;
    public string AllowedRoles { get; set; } = string.Empty;
}
