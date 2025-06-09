using System;
using System.Collections.Generic;

namespace JobApplicationPortal.Models;

public partial class Application
{
    public int Id { get; set; }

    public int CandidateId { get; set; }

    public int JobId { get; set; }

    public int Experience { get; set; }

    public string? NoteForEmployer { get; set; }

    public string CoverLetter { get; set; } = null!;

    public DateTime? AppliedDate { get; set; }

    public int StatusId { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Job Job { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
