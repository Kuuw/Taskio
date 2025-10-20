namespace Entities.Models;

public partial class Task
{
    public Guid TaskId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid CategoryId { get; set; }

    public string TaskName { get; set; } = null!;

    public string? TaskDesc { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int SortOrder { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
