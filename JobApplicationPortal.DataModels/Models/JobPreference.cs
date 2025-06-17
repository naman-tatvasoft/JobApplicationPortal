using System;
using System.Collections.Generic;

namespace JobApplicationPortal.DataModels.Models;

public partial class JobPreference
{
    public int Id { get; set; }

    public int CandidateId { get; set; }

    public int CategoryId { get; set; }

    public int? ExperienceRequired { get; set; }

    public string Location { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;
}
