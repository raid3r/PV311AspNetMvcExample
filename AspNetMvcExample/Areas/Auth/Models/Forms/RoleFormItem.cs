namespace AspNetMvcExample.Areas.Auth.Models.Forms;

public class RoleFormItem
{
    public int Id { get; set; }
    
    public string? Name { get; set; }
    public bool IsSelected { get; set; }
}