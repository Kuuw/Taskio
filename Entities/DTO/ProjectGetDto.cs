namespace Entities.DTO;

public partial class ProjectGetDto
{
    public Guid ProjectId { get; set; }

    public string ProjectName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CategoryGetDto> Categories { get; set; } = new List<CategoryGetDto>();
}
