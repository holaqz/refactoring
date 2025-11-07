namespace CourierManagementSystem.Api.Exceptions;

public class ValidationException : Exception
{
    public Dictionary<string, List<string>> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, List<string>>
        {
            { "general", new List<string> { message } }
        };
    }

    public ValidationException(Dictionary<string, List<string>> errors) : base("Validation failed")
    {
        Errors = errors;
    }
}
