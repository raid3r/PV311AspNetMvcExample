using AspNetMvcExample.Areas.Auth.Models.Forms;
using AspNetMvcExample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AspNetMvcExample.Areas.Auth.Controllers;

[Area("Auth")]
public class AccountController(
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    SiteContext context
    ) : Controller
{

    [HttpGet]
    public async Task<IActionResult> Register(string returnUrl = "")
    {
        return View(new RegisterForm());
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromForm] RegisterForm form, string returnUrl = "")
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        if (await userManager.FindByEmailAsync(form.Login) != null)
        {
            ModelState.AddModelError(nameof(form.Login), "User already exists");
            return View(form);
        }

        var user = new User()
        {
            Email = form.Login,
            UserName = form.Login,
        };


        var result = await userManager.CreateAsync(user, form.Password);

        if (!result.Succeeded)
        {
            /// result.Errors
            ModelState.AddModelError(nameof(form.Login), "Error .... (password)");
            return View(form);
        }

        await signInManager.SignInWithClaimsAsync(user, true,
        [
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("Avatar", user.Image?.Src ?? "")
        ]);

        //var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
        //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        //identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        //identity.AddClaim(new Claim("Avatar", user.Image?.Src ?? ""));

        //var userRoles = await _userManager.GetRolesAsync(user);
        //userRoles.ToList().ForEach(r =>
        //{
        //    identity.AddClaim(new Claim(ClaimTypes.Role, r));
        //});

        //var principal = new ClaimsPrincipal(identity);

        // Авторизувати користувача як новоствореного користувача
        //await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);


        return Redirect(returnUrl);
    }






    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl = "")
    {
        return View(new LoginForm());
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginForm form, string returnUrl = "")
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        //var user = await userManager.FindByEmailAsync(form.Login);
        var user = await context.Users
            .Include(x => x.Image)
            .FirstOrDefaultAsync(x => x.UserName == form.Login);

        if (user == null)
        {
            ModelState.AddModelError(nameof(form.Login), "User not found");
            return View(form);
        }

        //if (!await userManager.CheckPasswordAsync(user, form.Password))
        //{
        //    ModelState.AddModelError(nameof(form.Login), "Wrong password");
        //    return View(form);
        //}

        var result = await signInManager.CheckPasswordSignInAsync(user, form.Password, false);
        if (! result.Succeeded) { 
            ModelState.AddModelError(nameof(form.Login), "Wrong password");
            return View(form);
        }

        await signInManager.SignInWithClaimsAsync(user, true,
        [
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("Avatar", user.Image?.Src ?? "")
        ]);


        //var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
        //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        //identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        //identity.AddClaim(new Claim("Avatar", user.Image?.Src ?? ""));

        ////var userRoles = await _userManager.GetRolesAsync(user);
        ////userRoles.ToList().ForEach(r =>
        ////{
        ////    identity.AddClaim(new Claim(ClaimTypes.Role, r));
        ////});

        //var principal = new ClaimsPrincipal(identity);

        //// Авторизувати користувача як новоствореного користувача
        //await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

        if (String.IsNullOrEmpty(returnUrl))
        {
            return RedirectToAction("Index", new { Area = "", Controller = "Home" });
        }
        return Redirect(returnUrl);
    }

    // registration


    // login

    // logout
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToAction("Index", new { Area = "", Controller = "Home" });
    }
}
