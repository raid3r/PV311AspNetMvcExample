using System.ComponentModel.DataAnnotations;

namespace AspNetMvcExample.Models;

public class Review
{
    public int Id { get; set; }
    
    public virtual UserInfo UserInfo { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    
    
    public DateTime CreatedAt { get; set; }
    
    public int Rating { get; set; }
    
    [MaxLength(500)]
    public string Text { get; set; } = null!;
}