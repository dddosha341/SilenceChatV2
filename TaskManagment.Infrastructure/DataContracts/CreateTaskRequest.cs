namespace TaskManagement.Infrastructure.DataContracts;

public class CreateTaskRequest
{
    public required string Title { get; set; }

    public string? Description { get; set; }
}

