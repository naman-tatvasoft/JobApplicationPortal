﻿using System;
using System.Collections.Generic;

namespace JobApplicationPortal.Models;

public partial class Status
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Application> Applications { get; } = new List<Application>();
}
