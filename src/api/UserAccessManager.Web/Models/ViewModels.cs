namespace UserAccessManager.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public string? Message { get; set; }
}

public class DashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalApplications { get; set; }
    public int PendingRequests { get; set; }
    public List<AccessRequestDto> RecentRequests { get; set; } = [];
}
