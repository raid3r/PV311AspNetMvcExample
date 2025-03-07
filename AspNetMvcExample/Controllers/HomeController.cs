using AspNetMvcExample.Models;
using AspNetMvcExample.Models.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AspNetMvcExample.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    SiteContext context
) : Controller
{
    //     /Home/Index
    public IActionResult Index([FromQuery] SearchForm searchForm)
    {
        ViewData["userInfos"] = context
            .UserInfos
            .Include(x => x.Profession)
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .ToList();
        return View();
    }

    public IActionResult Search([FromQuery] SearchForm searchForm)
    {
        IQueryable<UserInfo> q = context
            .UserInfos
            .Include(x => x.Profession)
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.Reviews)
            .ThenInclude(x => x.User)
            .Include(x => x.MainImageFile);

        
        
        
        if (!string.IsNullOrEmpty(searchForm.Text))
        {
            q = q.Where(x => EF.Functions.Like(x.Email.ToUpper(), $"%{searchForm.Text.ToUpper()}%"));
        }

        int? userId = User.Identity.IsAuthenticated ? GetUserId() : null;
        ViewData["userId"] = userId;
        
        return PartialView("_SearchResult",
            q.ToList()
        );
    }

    //     /Home/Privacy
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Hello()
    {
        return View();
    }

    //     /Home/Hello
    //public IActionResult Hello()
    //{
    //    var userInfo = new UserInfoForm()
    //    {
    //        Birthday = DateTime.Now,
    //        Name = "Vasyl",
    //        Email = "test@test.com",
    //        Description = "Description",
    //        ExpirienseYears = 8,
    //        IsActive = true,
    //        Salary = 9999999.99M,
    //        Skills = [
    //            new Skill() {
    //            Title = "C#",
    //            Level = 99,
    //            },
    //            new Skill() {
    //            Title = "HTML",
    //            Level = 80,
    //            },
    //            new Skill() {
    //            Title = "CSS",
    //            Level = 60,
    //            },
    //            new Skill
    //            {
    //                Title = "ASP.NET",
    //                Level = 20,
    //            }
    //        ]
    //    };

    //    return View(userInfo);
    //}


    [Authorize]
    public async Task<IActionResult> AddReview(int id, [FromBody] ReviewForm reviewForm)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return Json(new { Ok = false, Error = "Invalid data" });
        }

        var model = new Review();
        model.Text = reviewForm.Text;
        model.Rating = reviewForm.Rating;
        model.UserInfo = await context.UserInfos.FirstAsync(x => x.Id == id);
        model.CreatedAt = DateTime.Now;

        var userId = GetUserId();
        model.User = await context.Users.FirstAsync(x => x.Id == userId);

        context.Reviews.Add(model);
        await context.SaveChangesAsync();
        return Json(new { Ok = true });
    }

    private int GetUserId()
    {
        return int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}