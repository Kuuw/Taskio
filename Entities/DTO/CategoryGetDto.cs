namespace Entities.DTO;

public partial class CategoryGetDto
{
    public Guid CategoryId { get; set; }

    public Guid ProjectId { get; set; }

    public string CategoryName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int SortOrder { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

}