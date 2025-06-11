using System;
using System.Collections.Generic;

namespace JobApplicationPortal.DataModels.Models;

public partial class Skill
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<JobSkill> JobSkills { get; } = new List<JobSkill>();
}
