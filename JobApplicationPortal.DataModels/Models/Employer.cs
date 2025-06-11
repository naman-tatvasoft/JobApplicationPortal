using System;
using System.Collections.Generic;

namespace JobApplicationPortal.DataModels.Models;

public partial class Employer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<Job> Jobs { get; } = new List<Job>();

    public virtual User User { get; set; } = null!;
}
