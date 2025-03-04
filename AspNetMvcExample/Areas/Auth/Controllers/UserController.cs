using AspNetMvcExample.Areas.Auth.Models.Forms;
using AspNetMvcExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetMvcExample.Areas.Auth.Controllers;

[Area("Auth")]
[Authorize(Roles = "Admin")]
public class UserController(
    UserManager<User> userManager, 
    RoleManager<IdentityRole<int>> roleManager
    ) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewData["userManager"] = userManager;
        return View(await userManager.Users
            .Include(x => x.Image)
            .ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        
        SetProfileForm(user);
        await SetUserRolesForm(user);
        ViewData["resetPasswordForm"] = new ResetPasswordForm();

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] ProfileForm profileForm)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        
        if (!ModelState.IsValid)
        {
            await SetUserRolesForm(user);
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
            Phone = user.PhoneNumber
        };
        ViewData["resetPasswordForm"] = resetPasswordForm;
        
        var allRoles = await roleManager.Roles.ToListAsync();
        var userRoles = await userManager.GetRolesAsync(user);
        var rolesForm = new RolesForm()
        {
            Roles = allRoles.Select(r => new RoleFormItem()
            {
                Id = r.Id,
                IsSelected = userRoles.Contains(r.Name),
                Name = r.Name,
            }).ToList()
        };
        ViewData["rolesForm"] = rolesForm;

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

    private async Task SetUserRolesForm(User user)
    {
        var allRoles = await roleManager.Roles.ToListAsync();
        var userRoles = await userManager.GetRolesAsync(user);
        ViewData["rolesForm"] = new RolesForm()
        {
            Roles = allRoles.Select(r => new RoleFormItem()
            {
                Id = r.Id,
                IsSelected = userRoles.Contains(r.Name),
                Name = r.Name,
            }).ToList()
        };
    }
    
    private void SetProfileForm(User user)
    {
        ViewData["profileForm"] = new ProfileForm()
        {
            FullName = user.FullName,
            Phone = user.PhoneNumber
        };
    }
    
    [HttpPost]
    public async Task<IActionResult> ChangeRoles(int id, [FromForm] RolesForm rolesForm)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        
        SetProfileForm(user);
        ViewData["resetPasswordForm"] = new ResetPasswordForm();
        await SetUserRolesForm(user);
        
        if (!ModelState.IsValid)
        {
            return View("Edit", user);
        }
        
        var allRoles = await roleManager.Roles.ToListAsync();
        var userRoles = await userManager.GetRolesAsync(user);

        foreach (var roleFormItem in rolesForm.Roles)
        {
            var roleName = allRoles.First(r => r.Id == roleFormItem.Id).Name;
            switch (roleFormItem.IsSelected)
            {
                case true when !await userManager.IsInRoleAsync(user, roleName):
                    await userManager.AddToRoleAsync(user, roleName);
                    break;
                case false when userRoles.Contains(roleName):
                    await userManager.RemoveFromRoleAsync(user, roleName);
                    break;
            }
        }
        
        TempData["SuccessMessage"] = "Roles changed successfully.";
        return RedirectToAction(nameof(Index));
    }
}