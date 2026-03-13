using System.ComponentModel.DataAnnotations;

namespace UserAccessManager.Web.Models;

public class UserDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string Source { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}

public class CreateUserRequest
{
    [Required, MaxLength(150)]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? FullName { get; set; }

    [EmailAddress, MaxLength(300)]
    public string? Email { get; set; }

    [Required, MaxLength(20)]
    public string Source { get; set; } = string.Empty;

    public string? CreatedBy { get; set; }
}

public class UpdateUserRequest
{
    [MaxLength(200)]
    public string? FullName { get; set; }

    [EmailAddress, MaxLength(300)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Source { get; set; }

    public string? UpdatedBy { get; set; }
}
