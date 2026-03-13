namespace UserAccessManager.Core.DTOs.Response;

public class ApplicationDto
{
    public int AppId { get; set; }
    public string AppName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
}
