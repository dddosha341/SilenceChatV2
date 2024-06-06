namespace TaskManagement.Infrastructure.DataContracts;

public class TaskEntity
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }

    public TaskStatus Status { get; set; }

    public required UserEntity Owner { get; set; }
    public UserEntity? AssignedUser { get; set; }
    public TeamEntity? Team { get; set; }
}

