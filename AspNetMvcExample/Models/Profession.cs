using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetMvcExample.Models;

[Table("Profession")]
public class Profession
{
    public int Id { get; set; }
    public string Title { get; set; }
}
