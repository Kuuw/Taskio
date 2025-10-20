namespace Entities.Models;

public partial class ProjectUser
{
    public Guid ProjectId { get; set; }

    public Guid UserId { get; set; }

    public bool IsAdmin { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
