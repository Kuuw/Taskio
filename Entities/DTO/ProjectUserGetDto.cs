namespace Entities.DTO;

public partial class ProjectUserGetDto
{
    public Guid UserId { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public bool IsAdmin { get; set; }
}
