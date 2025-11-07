namespace CourierManagementSystem.Api.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, long id) : base($"{entityName} with ID {id} not found")
    {
    }
}
