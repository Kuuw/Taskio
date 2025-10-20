namespace Entities.DTO;

public partial class CategoryPostDto
{
    public Guid ProjectId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int SortOrder { get; set; }
}