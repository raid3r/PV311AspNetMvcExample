namespace AspNetMvcExample.Models.Forms;

public class ReviewForm
{
    public int Rating { get; set; }
    
    public string Text { get; set; } = null!;
}