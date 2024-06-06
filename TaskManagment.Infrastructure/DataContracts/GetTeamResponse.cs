namespace TaskManagement.Infrastructure.DataContracts;

public class GetTeamResponse : TeamEntity
{
    public IEnumerable<UserEntity> Users { get; set; }
}

