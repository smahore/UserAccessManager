namespace UserAccessManager.Core.DTOs.Response;

public class AccessRequestDto
{
    public int AccessRequestId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public int AppRoleId { get; set; }
    public string? Email { get; set; }
}
