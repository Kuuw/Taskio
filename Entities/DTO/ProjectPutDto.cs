namespace Entities.DTO;

public partial class ProjectPutDto
{
    public Guid ProjectId { get; set; }

    public string? ProjectName { get; set; } = null!;
}
