using System;
using System.Collections.Generic;

namespace JobApplicationPortal.DataModels.Models;

public partial class Candidate
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<Application> Applications { get; } = new List<Application>();

    public virtual ICollection<JobPreference> JobPreferences { get; } = new List<JobPreference>();

    public virtual User User { get; set; } = null!;
}
