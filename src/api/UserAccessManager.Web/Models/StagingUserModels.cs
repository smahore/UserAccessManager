namespace UserAccessManager.Web.Models;

public class StagingUserDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Source { get; set; }
}

public class PromoteUserRequest
{
    public string? CreatedBy { get; set; }
}
