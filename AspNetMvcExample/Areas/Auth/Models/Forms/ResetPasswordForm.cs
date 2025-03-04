namespace AspNetMvcExample.Areas.Auth.Models.Forms;

public class ResetPasswordForm
{
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}