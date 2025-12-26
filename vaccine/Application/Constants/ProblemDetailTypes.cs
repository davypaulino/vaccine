namespace vaccine.Application.Constants;

public class ProblemDetailTypes
{ 
    /// <summary>
    /// The request is invalid or cannot be processed due to validation errors.
    /// </summary>
    public const string Validation =
        "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1";

    /// <summary>
    /// The request could not be understood or was invalid.
    /// </summary>
    public const string BadRequest =
        "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1";

    /// <summary>
    /// The requested resource was not found.
    /// </summary>
    public const string NotFound =
        "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5";

    /// <summary>
    /// An unexpected server error occurred.
    /// </summary>
    public const string InternalError =
        "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";
}