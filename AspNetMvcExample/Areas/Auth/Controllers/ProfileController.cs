using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetMvcExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AspNetMvcExample.Areas.Auth.Models.Forms;
using AspNetMvcExample.Models.Services;
using AspNetMvcExample.Migrations;

namespace AspNetMvcExample.Areas.Auth.Controllers;

[Route("profile")]
[Area("Auth")]
[Authorize]
public class ProfileController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    SiteContext context,
    FileStorage fileStorage
) : Controller
{
    private async Task<User> GetCurrentUserAsync()
    {
        var id = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
        return await context.Users
            .Include(x => x.Image)
            .FirstAsync(x => x.Id == id);
    }

    [HttpGet("index")]
    public async Task<IActionResult> Index()
    {
        return View(await GetCurrentUserAsync());
    }

    [HttpGet("edit")]
    public async Task<IActionResult> Edit()
    {
        var user = await GetCurrentUserAsync();

        return View(new ProfileForm
        {
            FullName = user.FullName,
            Phone = user.PhoneNumber,
        });
    }

    [HttpPost("edit")]
    public async Task<IActionResult> Edit([FromForm] ProfileForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var model = await GetCurrentUserAsync();
        model.FullName = form.FullName;
        model.PhoneNumber = form.Phone;

        if (form.Image != null)
        {
            if (model.Image != null)
            {
                fileStorage.Delete(model.Image);
                context.ImageFiles.Remove(model.Image);
            }

            model.Image = await fileStorage.SaveAsync(form.Image);
        }

        await context.SaveChangesAsync();


        await signInManager.SignOutAsync();
        await signInManager.SignInWithClaimsAsync(model, true,
        [
            new Claim(ClaimTypes.Email, model.Email),
            new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()),
            new Claim("Avatar", model.Image?.Src ?? "")
        ]);

        return RedirectToAction("Index");
    }

    [HttpGet("change-password")]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordForm());
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        if (form.NewPassword != form.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(form.ConfirmPassword),
                "New password and confirmation password do not match.");
            return View(form);
        }

        var user = await GetCurrentUserAsync();
        var result = await userManager.ChangePasswordAsync(user, form.Password, form.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(nameof(form.Password), error.Description);
            }

            return View(form);
        }

        await signInManager.SignOutAsync();

        var claims = new List<Claim>()
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("Avatar", user.Image?.Src ?? "")
        };

        var userRoles = await userManager.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));

        await signInManager.SignInWithClaimsAsync(user, true, claims);

        return RedirectToAction("Index");
    }

    [HttpDelete("api/delete")]
    public async Task<IActionResult> Delete()
    {
        await userManager.DeleteAsync(await GetCurrentUserAsync());
        await signInManager.SignOutAsync();
        return Json(new { success = true });
    }
}