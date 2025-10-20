namespace Entities.Models;

public partial class Project
{
    public Guid ProjectId { get; set; }

    public string ProjectName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
