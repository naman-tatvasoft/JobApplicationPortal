using System;
using System.Collections.Generic;

namespace JobApplicationPortal.DataModels.Models;

public partial class Job
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int? ExperienceRequired { get; set; }

    public int EmployerId { get; set; }

    public DateOnly OpenFrom { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long Vacancy { get; set; }

    public virtual ICollection<Application> Applications { get; } = new List<Application>();

    public virtual Employer Employer { get; set; } = null!;

    public virtual ICollection<JobSkill> JobSkills { get; } = new List<JobSkill>();
}
