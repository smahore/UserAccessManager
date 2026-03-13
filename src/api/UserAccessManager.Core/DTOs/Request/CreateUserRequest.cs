namespace UserAccessManager.Core.DTOs.Request;

public class CreateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string Source { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = "API";
}
