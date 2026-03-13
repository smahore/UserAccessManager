namespace UserAccessManager.Core.DTOs.Response;

/// <summary>
/// Outcome of an access-request status update, including fulfillment results.
/// </summary>
public record StatusUpdateResult(bool Success, string Message)
{
    public static StatusUpdateResult NotFound(int id) =>
        new(false, $"Access request with ID {id} not found.");

    public static StatusUpdateResult UserNotFound(string email) =>
        new(false, $"Cannot fulfill: no active user found for '{email}'.");

    public static StatusUpdateResult UserInactive(string email) =>
        new(false, $"Cannot fulfill: user '{email}' is inactive.");

    public static StatusUpdateResult RoleAlreadyAssigned(string roleName) =>
        new(false, $"Cannot fulfill: user already has the '{roleName}' role assigned.");

    public static StatusUpdateResult Updated(string status) =>
        new(true, $"Access request status updated to '{status}'.");

    public static StatusUpdateResult Fulfilled(string roleName) =>
        new(true, $"Access request fulfilled and '{roleName}' role granted.");
}
