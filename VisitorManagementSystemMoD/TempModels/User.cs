using System;
using System.Collections.Generic;

namespace VisitorManagementSystemMoD.TempModels;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? RoleId { get; set; }

    public int? DepartmentId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Username { get; set; } = null!;

    public virtual Department? Department { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<Visitor> VisitorApprovedBies { get; set; } = new List<Visitor>();

    public virtual ICollection<Visitor> VisitorEmployees { get; set; } = new List<Visitor>();
}
