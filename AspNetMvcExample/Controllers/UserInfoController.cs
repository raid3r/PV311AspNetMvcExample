using AspNetMvcExample.Models;
using AspNetMvcExample.Models.Forms;
using AspNetMvcExample.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AspNetMvcExample.Controllers;

[Authorize(Roles = "Admin,User")]
[Route("user-infos-data")]
public class UserInfoController(
    ILogger<UserInfoController> logger,
    SiteContext context,
    FileStorage fileStorage
    ) : Controller
{
    // /UserInfo/Index
    /// <summary>
    /// Перегляд списку усіх наявних
    /// </summary>
    /// <returns></returns>
    [HttpGet("list")]
    public IActionResult Index()
    {
        return View(
            context.UserInfos
            .Include(x => x.Profession)
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .ToList()
            );
    }

    /// <summary>
    /// Перегляд інформації по одному елементу
    /// </summary>
    /// <param name="id">Ід потрібного елементу</param>
    /// <returns></returns>
    [HttpGet("{id:int}/view")]
    public IActionResult View(int id) // 1 2
    {
        return View(
            context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .First(x => x.Id == id)
            );
    }

    /// <summary>
    /// Перегляд інформації по одному елементу
    /// </summary>
    /// <param name="id">Ід потрібного елементу</param>
    /// <returns></returns>
    [HttpGet("{id:int}/view")]
    public IActionResult ViewPartial(int id) // 1 2
    {
        return PartialView(
            "View",
            context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .First(x => x.Id == id)
            );
    }

    [HttpGet("create-info")]
    public IActionResult Create()
    {
        var model = new UserInfoForm(new UserInfo());
        model.Professions = context.Professions.ToList();
        return View(model);
    }

    [HttpPost("create-info")]
    public async Task<IActionResult> Create([FromForm] UserInfoForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }
        var model = new UserInfo();
        form.Update(model);

        if (form.Gallery != null)
        {
            foreach (var item in form.Gallery)
            {
                var imageFile = await fileStorage.SaveAsync(item);
                model.ImageFiles.Add(imageFile);
            }
            model.MainImageFile = model.ImageFiles.First();
        }

        context.UserInfos.Add(model);
        await context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["id"] = id;
        var model = await context.UserInfos
            .Include(x => x.Profession)
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .ThenInclude(x => x.ImageFile)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .FirstAsync(x => x.Id == id);
        
        var form = new UserInfoForm(model);
        form.Professions = await context.Professions.ToListAsync();
        
        var userSkills = model.UserSkills;
        var skills = await context.Skills
            .Include(x => x.ImageFile)
            .ToListAsync();
        var availableSkills = skills
            .Where(x => !userSkills.Select(x => x.Skill.Id)
            .ToList().Contains(x.Id))
            .ToList();

        ViewData["userSkills"] = userSkills;
        ViewData["skills"] = skills;
        ViewData["availableSkills"] = availableSkills;

        return View(form);
    }

    [HttpPost("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id, [FromForm] UserInfoForm form)
    {

        var model = await context.UserInfos
            .Include(x => x.Profession)
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .FirstAsync(x => x.Id == id);


        if (!ModelState.IsValid)
        {
            ViewData["id"] = id;


            var userSkills = model.UserSkills;
            var skills = await context.Skills
                .Include(x => x.ImageFile)
                .ToListAsync();
            var availableSkills = skills
                .Where(x => !userSkills.Select(x => x.Skill.Id)
                .ToList().Contains(x.Id))
                .ToList();

            ViewData["userSkills"] = userSkills;
            ViewData["skills"] = skills;
            ViewData["availableSkills"] = availableSkills;

            form.Professions = await context.Professions.ToListAsync();
            return View(form);
        }

        if (form.Gallery != null)
        {
            foreach (var item in form.Gallery)
            {
                var imageFile = await fileStorage.SaveAsync(item);
                model.ImageFiles.Add(imageFile);
            }
        }

        form.Update(model);
        model.Profession = await context.Professions.FirstAsync(x => x.Id == form.ProfessionId);


        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }


    [HttpGet("change-main-image")]
    public async Task<IActionResult> ChangeMainImage(int id, [FromQuery] int imageId)
    {
        var model = await context.UserInfos
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .FirstAsync(x => x.Id == id);

        model.MainImageFile = model.ImageFiles.First(x => x.Id == imageId);
        await context.SaveChangesAsync();

        return Json(new { Ok = true });
    }

    [HttpDelete("{id:int}/delete-skill")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var userSkill = await context.UserSkills.FirstAsync(x => x.Id == id);
        context.UserSkills.Remove(userSkill);
        await context.SaveChangesAsync();
        return Json(new { Ok = true });
    }

    [HttpPost("{id:int}/add-skill")]
    public async Task<IActionResult> AddSkill(int id, [FromBody] UserSkillForm data)
    {
        var user = await context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .FirstAsync(x => x.Id == id);

        //TODO form
        var skill = await context.Skills.FirstAsync(x => x.Id == data.SkillId);
        

        if (null != user.UserSkills.FirstOrDefault(x => x.Skill.Id == skill.Id))
        {
            // Already added
            Response.StatusCode = 400;
            return Json(new { Ok = false, Error = "Alredy exists" });
        }

        user.UserSkills.Add(new UserSkill
        {
            Level = data.Level,
            Skill = skill,
            UserInfo = user
        });

        await context.SaveChangesAsync();

        return Json(new { Ok = true });
    }


    [HttpGet("{id:int}/edit-with-js")]
    public async Task<IActionResult> EditV2(int id)
    {
        ViewData["id"] = id;

        var model = await context.UserInfos
            .Include(x => x.Profession)
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .ThenInclude(x => x.ImageFile)
            .FirstAsync(x => x.Id == id);

        var form = new UserInfoForm(model);

        var userSkills = model.UserSkills;
        var skills = await context.Skills
            .Include(x => x.ImageFile)
            .ToListAsync();

        var userSkillForms = skills.Select(s =>
        {
            var userSkill = userSkills.FirstOrDefault(x => x.Skill.Id == s.Id);
            return new UserSkillForm
            {
                SkillId = s.Id,
                Selected = userSkill != null,
                Level = userSkill?.Level ?? 0,
            };
        }).ToList();

        ViewData["userSkillForms"] = userSkillForms;
        ViewData["skills"] = skills;

        return View("EditWithoutJS", form);
    }

    [HttpPost("{id:int}/edit-with-js")]
    public async Task<IActionResult> EditV2(int id, [FromForm] UserInfoForm form, [FromForm] List<UserSkillForm>? userSkillForms)
    {
        var skills = await context.Skills.ToListAsync();
        if (!ModelState.IsValid)
        {
            ViewData["id"] = id;
            ViewData["userSkillForms"] = userSkillForms;
            ViewData["skills"] = skills;
            return View("EditWithoutJS", form);
        }

        var model = await context.UserInfos
            .Include(x => x.Profession)
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .FirstAsync(x => x.Id == id);

        if (form.Gallery != null)
        {
            foreach (var item in form.Gallery)
            {
                var imageFile = await fileStorage.SaveAsync(item);
                model.ImageFiles.Add(imageFile);
            }
        }

        form.Update(model);
        model.Profession = await context.Professions.FirstAsync(x => x.Id == form.ProfessionId);
        
        var userSkills = model.UserSkills;
        foreach (var item in userSkillForms)
        {
            var userSkill = userSkills.FirstOrDefault(x => x.Skill.Id == item.SkillId);
            if (item.Selected)
            {
                if (userSkill == null)
                {
                    var newUserSkill = new UserSkill
                    {
                        UserInfo = model,
                        Skill = skills.First(x => x.Id == item.SkillId),
                        Level = item.Level,
                    };

                    model.UserSkills.Add(newUserSkill);
                }
                else
                {
                    userSkill.Level = item.Level;
                }
            }
            else
            {
                if (userSkill != null)
                {
                    model.UserSkills.Remove(userSkill);
                }
            }
        }
        await context.SaveChangesAsync();

        return RedirectToAction("Index");
    }


}
