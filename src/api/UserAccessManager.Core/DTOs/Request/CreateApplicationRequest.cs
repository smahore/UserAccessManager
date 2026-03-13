namespace UserAccessManager.Core.DTOs.Request;

public class CreateApplicationRequest
{
    public string AppName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
