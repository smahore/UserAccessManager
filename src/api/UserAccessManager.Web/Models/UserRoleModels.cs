using System.ComponentModel.DataAnnotations;

namespace UserAccessManager.Web.Models;

public class UserRoleDto
{
    public int UserRoleId { get; set; }
    public int UserId { get; set; }
    public int AppId { get; set; }
    public string? AppName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AssignRoleRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Please select an application.")]
    public int AppId { get; set; }
}
