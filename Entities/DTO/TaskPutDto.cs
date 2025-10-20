namespace Entities.DTO;

public partial class TaskPutDto
{
    public Guid TaskId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid CategoryId { get; set; }

    public string TaskName { get; set; } = null!;

    public string? TaskDesc { get; set; }

    public DateTime? DueDate { get; set; }
}
