﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetMvcExample.Models;

public class UserInfo
{
    public UserInfo()
    {
        ImageFiles = new List<ImageFile>();
        UserSkills = new List<UserSkill>();
        Reviews = new List<Review>();
    }

    public int Id { get; set; }

    [MaxLength(100)] public string Name { get; set; } = null!;

    [MaxLength(100)] public string? Email { get; set; }

    [MaxLength(1000)] public string? Description { get; set; }

    public DateTime Birthday { get; set; }

    public bool IsActive { get; set; }

    public int ExpirienseYears { get; set; }

    public decimal Salary { get; set; }

    public virtual Profession? Profession { get; set; }

    public virtual ICollection<ImageFile> ImageFiles { get; set; }

    public virtual ICollection<UserSkill> UserSkills { get; set; }

    public virtual ImageFile? MainImageFile { get; set; }

    public virtual User? Author { get; set; } = null!;
    
    public virtual ICollection<Review> Reviews { get; set; }
}