using System.Collections.Generic;

namespace GymManagment.BLL.Common;

/// <summary>
/// Represents the outcome of a service operation.
/// On failure it carries a dictionary of field-name → error-message pairs
/// so the controller can push them straight into ModelState.
/// </summary>

//Any Service method that needs to return success/failure + business-rule errors can return this type. 

public class ServiceResult
{
    public bool Succeeded { get; private set; }

    // Key = model property name (e.g. "Email", "Phone"), Value = error message
    public Dictionary<string, string> Errors { get; private set; } = new();

    // ── factory helpers ──────────────────────────────────────────────────────

    public static ServiceResult Success() => new() { Succeeded = true };

    //one error only
    public static ServiceResult Failure(string field, string message)
    {
        var result = new ServiceResult { Succeeded = false };
        result.Errors[field] = message;
        return result;
    }

    //multiple errors
    public static ServiceResult Failure(Dictionary<string, string> errors)
    {
        var result = new ServiceResult { Succeeded = false };
        result.Errors = errors;
        return result;
    }
}
