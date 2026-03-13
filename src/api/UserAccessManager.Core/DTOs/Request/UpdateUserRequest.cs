namespace UserAccessManager.Core.DTOs.Request;

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Source { get; set; }
    public string UpdatedBy { get; set; } = "API";
}
