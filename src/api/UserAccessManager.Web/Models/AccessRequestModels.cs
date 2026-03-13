using System.ComponentModel.DataAnnotations;

namespace UserAccessManager.Web.Models;

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

public class AccessRequestResultDto
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int AppRoleId { get; set; }
    public string? RoleName { get; set; }
}

public class CreateAccessRequest
{
    [Required, MaxLength(255)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Please select a role.")]
    public int AppRoleId { get; set; }
}

public class UpdateStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
