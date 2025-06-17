using System;
using System.Collections.Generic;

namespace JobApplicationPortal.DataModels.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<JobPreference> JobPreferences { get; } = new List<JobPreference>();

    public virtual ICollection<Job> Jobs { get; } = new List<Job>();
}
