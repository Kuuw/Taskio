namespace Entities.DTO;

public partial class CategoryPutDto
{
    public Guid CategoryId { get; set; }

    public Guid ProjectId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int SortOrder { get; set; }
}