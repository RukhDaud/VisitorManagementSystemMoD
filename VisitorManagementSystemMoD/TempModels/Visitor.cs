using System;
using System.Collections.Generic;

namespace VisitorManagementSystemMoD.TempModels;

public partial class Visitor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Cnic { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Purpose { get; set; } = null!;

    public DateTime ExpectedTime { get; set; }

    public bool HasVehicle { get; set; }

    public string? VehicleNumber { get; set; }

    public string? VehicleType { get; set; }

    public string Status { get; set; } = null!;

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public int? ApprovedById { get; set; }

    public string? ApprovedByName { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public DateTime RequestCreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? ApprovedBy { get; set; }

    public virtual Department? Department { get; set; }

    public virtual User Employee { get; set; } = null!;
}
