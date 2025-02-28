namespace AspNetMvcExample.Areas.Auth.Models.Forms;

public class ChangePasswordForm
{
    public string Password { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}