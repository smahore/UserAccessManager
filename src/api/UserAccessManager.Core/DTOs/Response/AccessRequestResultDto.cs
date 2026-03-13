namespace UserAccessManager.Core.DTOs.Response;

public class AccessRequestResultDto
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? AppRoleId { get; set; }
    public string? RoleName { get; set; }
}
