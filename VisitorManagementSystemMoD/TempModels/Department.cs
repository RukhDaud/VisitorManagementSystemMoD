using System;
using System.Collections.Generic;

namespace VisitorManagementSystemMoD.TempModels;

public partial class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Visitor> Visitors { get; set; } = new List<Visitor>();
}
