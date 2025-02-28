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

    public async Task<IActionResult> Index()
    {
        return View(await GetCurrentUserAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await GetCurrentUserAsync();

        return View(new ProfileForm
        {
            FullName = user.FullName,
            Phone = user.PhoneNumber,
        });
    }

    [HttpPost]
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

}
