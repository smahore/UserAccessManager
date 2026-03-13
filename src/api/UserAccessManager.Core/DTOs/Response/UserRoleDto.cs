namespace UserAccessManager.Core.DTOs.Response;

public class UserRoleDto
{
    public int UserRoleId { get; set; }
    public int UserId { get; set; }
    public int AppId { get; set; }
    public string? AppName { get; set; }
    public DateTime CreatedAt { get; set; }
}
