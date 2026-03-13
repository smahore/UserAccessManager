using System.ComponentModel.DataAnnotations;

namespace UserAccessManager.Web.Models;

public class ApplicationDto
{
    public int AppId { get; set; }
    public string AppName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateApplicationRequest
{
    [Required, MaxLength(100)]
    public string AppName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }
}

public class UpdateApplicationRequest
{
    [MaxLength(100)]
    public string? AppName { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }
}
