using AspNetMvcExample.Areas.Auth.Models.Forms;
using AspNetMvcExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetMvcExample.Areas.Auth.Controllers;

[Area("Auth")]
[Authorize(Roles = "Admin")]
public class UserController(UserManager<User> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await userManager.Users.Include(x => x.Image).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        var profileForm = new ProfileForm()
        {
            FullName = user.FullName,
            Phone = user.PhoneNumber,
        };
        var resetPasswordForm = new ResetPasswordForm();
        ViewData["profileForm"] = profileForm;
        ViewData["resetPasswordForm"] = resetPasswordForm;
        
        return View(user);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] ProfileForm profileForm)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (!ModelState.IsValid)
        {
            ViewData["profileForm"] = profileForm;
            ViewData["resetPasswordForm"] = new ResetPasswordForm();
            return View(user);
        }
        
        user.FullName = profileForm.FullName;
        user.PhoneNumber = profileForm.Phone;
        await userManager.UpdateAsync(user);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    public async Task<IActionResult> ResetPassword(int id, [FromForm] ResetPasswordForm resetPasswordForm)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        ViewData["profileForm"] = new ProfileForm()
        {
            FullName = user.FullName,
            Phone = user.PhoneNumber,
        };
        ViewData["resetPasswordForm"] = resetPasswordForm;
        
        if (!ModelState.IsValid)
        {
            return View(user);
        }
    
        if (resetPasswordForm.NewPassword != resetPasswordForm.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(resetPasswordForm.ConfirmPassword), "Passwords do not match.");
            return View("Edit", user);
        }
    
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordResult = await userManager.ResetPasswordAsync(user, resetToken, resetPasswordForm.NewPassword);
    
        if (!resetPasswordResult.Succeeded)
        {
            foreach (var error in resetPasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View("Edit", user);
        }
    
        TempData["SuccessMessage"] = "Password reset successful.";
        return RedirectToAction(nameof(Index));
    }
}