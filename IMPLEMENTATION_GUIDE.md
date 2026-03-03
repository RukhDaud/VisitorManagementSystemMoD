# Implementation Guide: Role-Based Access Control (RBAC)

## Quick Start

### 1. View the Roles & Permissions Documentation
Open and review the comprehensive documentation:
- **File:** `ROLES_AND_PERMISSIONS.md`
- Contains complete details about all roles, permissions, and usage examples

### 2. Access the Interactive Roles Guide
Navigate to: `/Account/RolesGuide`
- Visual permission matrix
- Role descriptions
- Your current permissions

---

## Using the Authorization System

### In Controllers

#### Restrict by Role
```csharp
using VisitorManagementSystemMoD.Attributes;
using VisitorManagementSystemMoD.Constants;

[RequireRole(Roles.SuperAdmin, Roles.Admin)]
public IActionResult ManageUsers()
{
    // Only SuperAdmin and Admin can access
    return View();
}
```

#### Restrict by Permission
```csharp
[RequirePermission(Permissions.ApproveVisitorRequest)]
public IActionResult ApproveVisitor(int id)
{
    // Only roles with approval permission can access
    return View();
}
```

#### Multiple Permissions
```csharp
[RequirePermission(
    Permissions.EditAnyVisitorRequest, 
    Permissions.EditPersonalVisitorRequest)]
public IActionResult EditVisitor(int id)
{
    // User needs at least ONE of these permissions
    return View();
}
```

### In Views (Razor)

#### Check Permissions
```razor
@using VisitorManagementSystemMoD.Constants
@inject VisitorManagementSystemMoD.Services.IAuthorizationService AuthService

@{
    var userRole = Context.Session.GetString("UserRole");
}

@if (AuthService.HasPermission(userRole, Permissions.CreateVisitorRequest))
{
    <a asp-action="Create" class="btn btn-primary">Create Visitor</a>
}
```

#### Check Roles
```razor
@if (Context.IsInRole(Roles.SuperAdmin, Roles.Admin))
{
    <div class="admin-panel">
        <!-- Admin-only content -->
    </div>
}
```

#### Using Extension Methods
```razor
@if (Context.HasPermission(Permissions.ApproveVisitorRequest))
{
    <button class="btn btn-success">Approve</button>
}

@if (Context.IsInRole(Roles.Reception))
{
    <div>Reception-specific content</div>
}
```

### In Code (C#)

#### Inject the Service
```csharp
private readonly IAuthorizationService _authService;

public YourController(IAuthorizationService authService)
{
    _authService = authService;
}
```

#### Check Permissions
```csharp
var userRole = HttpContext.Session.GetString("UserRole");

// Single permission
if (_authService.HasPermission(userRole, Permissions.EditAnyVisitorRequest))
{
    // Allow edit
}

// Multiple permissions (ANY)
if (_authService.HasAnyPermission(userRole, 
    Permissions.EditAnyVisitorRequest, 
    Permissions.EditPersonalVisitorRequest))
{
    // Allow edit if user has at least one permission
}

// Multiple permissions (ALL)
if (_authService.HasAllPermissions(userRole, 
    Permissions.CreateUser, 
    Permissions.AssignRoles))
{
    // Allow only if user has both permissions
}
```

#### HttpContext Extensions
```csharp
// In any controller
if (HttpContext.HasPermission(Permissions.ApproveVisitorRequest))
{
    // User has permission
}

if (HttpContext.IsInRole(Roles.SuperAdmin, Roles.Admin))
{
    // User is SuperAdmin or Admin
}

var permissions = HttpContext.GetUserPermissions();
// Returns List<string> of all user permissions
```

---

## Role Definitions

### SuperAdmin 👑
- **Full system access**
- Can manage users, roles, and departments
- All permissions granted

### Admin 🔐
- System oversight
- Can manage visitors and generate reports
- Cannot manage roles or users

### Security Officer 👮
- Approval management
- Can approve/reject requests
- Can generate visitor reports
- Cannot create or modify requests

### Employee 👤
- Personal request management
- Can create and manage own visitors
- Cannot see others' requests

### Reception 🚪
- Check-in/check-out operations
- Can process visitor entry/exit
- Limited reporting capabilities

---

## Permission Categories

### Visitor Management
- `ViewAllVisitorRequests`
- `ViewPersonalVisitorRequests`
- `CreateVisitorRequest`
- `EditPersonalVisitorRequest`
- `EditAnyVisitorRequest`
- `DeletePersonalVisitorRequest`
- `DeleteAnyVisitorRequest`

### Approvals
- `ApproveVisitorRequest`
- `RejectVisitorRequest`

### Check-In/Out
- `CheckInVisitor`
- `CheckOutVisitor`
- `ViewCheckInStatus`

### Reports
- `GenerateAllReports`
- `GenerateVisitorReports`
- `GenerateLimitedReports`
- `ViewCheckedInVisitors`
- `ViewCheckedOutVisitors`
- `ViewPendingVisitors`

### User Management
- `CreateUser`
- `EditUser`
- `DeleteUser`
- `ViewAllUsers`

### Role Management
- `CreateRole`
- `EditRole`
- `DeleteRole`
- `ViewAllRoles`
- `AssignRoles`

### Department Management
- `CreateDepartment`
- `EditDepartment`
- `DeleteDepartment`
- `ViewAllDepartments`

### System
- `AccessAdminPanel`
- `ManageSystemSettings`
- `ViewSystemLogs`

---

## Files Created

1. **Constants/RolePermissions.cs**
   - Defines all roles and permissions
   - Maps permissions to roles
   - Provides helper methods

2. **Attributes/AuthorizationAttributes.cs**
   - `[RequirePermission]` attribute
   - `[RequireRole]` attribute
   - Authorization filters

3. **Services/AuthorizationService.cs**
   - `IAuthorizationService` interface
   - Permission checking service
   - HttpContext extensions

4. **Views/Account/AccessDenied.cshtml**
   - Access denied page
   - User-friendly error message

5. **Views/Account/RolesGuide.cshtml**
   - Interactive roles guide
   - Permission matrix
   - Visual reference

6. **ROLES_AND_PERMISSIONS.md**
   - Comprehensive documentation
   - Usage examples
   - Best practices

---

## Testing the System

### 1. Test Role-Based Access
```csharp
// Create test action
[RequireRole(Roles.SuperAdmin)]
public IActionResult TestSuperAdminOnly()
{
    return Content("You have SuperAdmin access!");
}
```

### 2. Test Permission-Based Access
```csharp
[RequirePermission(Permissions.ApproveVisitorRequest)]
public IActionResult TestApprovalPermission()
{
    return Content("You can approve visitors!");
}
```

### 3. Test in Views
```razor
@if (Context.HasPermission(Permissions.CreateVisitorRequest))
{
    <div class="alert alert-success">
        You can create visitor requests!
    </div>
}
else
{
    <div class="alert alert-warning">
        You cannot create visitor requests.
    </div>
}
```

---

## Common Use Cases

### Example 1: Employee Creating Visitor
```csharp
[RequirePermission(Permissions.CreateVisitorRequest)]
public IActionResult Create()
{
    return View();
}

[HttpPost]
[RequirePermission(Permissions.CreateVisitorRequest)]
public async Task<IActionResult> Create(Visitor visitor)
{
    // Set employee ID from session
    visitor.EmployeeId = HttpContext.Session.GetInt32("UserId").Value;
    
    // Save visitor
    await _context.Visitors.AddAsync(visitor);
    await _context.SaveChangesAsync();
    
    return RedirectToAction("MyVisitors");
}
```

### Example 2: Security Officer Approving
```csharp
[RequirePermission(Permissions.ApproveVisitorRequest)]
public async Task<IActionResult> Approve(int id)
{
    var visitor = await _context.Visitors.FindAsync(id);
    if (visitor == null)
        return NotFound();
    
    visitor.Status = "Approved";
    visitor.ApprovedBy = HttpContext.Session.GetInt32("UserId");
    visitor.ApprovedAt = DateTime.Now;
    
    await _context.SaveChangesAsync();
    
    return RedirectToAction("PendingApprovals");
}
```

### Example 3: Reception Check-In
```csharp
[RequirePermission(Permissions.CheckInVisitor)]
public async Task<IActionResult> CheckIn(int id)
{
    var visitor = await _context.Visitors.FindAsync(id);
    if (visitor == null || visitor.Status != "Approved")
        return BadRequest();
    
    visitor.CheckInTime = DateTime.Now;
    visitor.CheckedInBy = HttpContext.Session.GetInt32("UserId");
    
    await _context.SaveChangesAsync();
    
    return RedirectToAction("GateVisitors");
}
```

### Example 4: Conditional UI Based on Role
```razor
<div class="dashboard">
    @if (Context.IsInRole(Roles.SuperAdmin, Roles.Admin))
    {
        <!-- Admin Dashboard -->
        <div class="admin-section">
            <h2>System Management</h2>
            
            @if (Context.HasPermission(Permissions.CreateUser))
            {
                <a asp-action="CreateUser" class="btn btn-primary">Create User</a>
            }
            
            @if (Context.HasPermission(Permissions.CreateRole))
            {
                <a asp-action="CreateRole" class="btn btn-success">Create Role</a>
            }
        </div>
    }
    
    @if (Context.IsInRole(Roles.Employee))
    {
        <!-- Employee Dashboard -->
        <div class="employee-section">
            <h2>My Visitors</h2>
            <a asp-action="Create" class="btn btn-primary">Create Visitor Request</a>
        </div>
    }
    
    @if (Context.IsInRole(Roles.Reception))
    {
        <!-- Reception Dashboard -->
        <div class="reception-section">
            <h2>Check-In/Out</h2>
            <!-- Reception specific content -->
        </div>
    }
</div>
```

---

## Troubleshooting

### Issue: Access Denied Even With Correct Role
**Solution:**
1. Clear session and log in again
2. Check if permission is mapped to role in `RolePermissions.cs`
3. Verify session has `UserRole` set correctly

### Issue: Permission Check Returns False
**Solution:**
1. Check spelling of permission constant
2. Ensure permission is in role's permission list
3. Verify user role is set in session

### Issue: Attribute Not Working
**Solution:**
1. Ensure `using VisitorManagementSystemMoD.Attributes;`
2. Check if session is enabled in Program.cs
3. Verify user is logged in

---

## Next Steps

1. **Review Documentation:** Read `ROLES_AND_PERMISSIONS.md`
2. **View Interactive Guide:** Navigate to `/Account/RolesGuide`
3. **Apply Attributes:** Add `[RequireRole]` or `[RequirePermission]` to controller actions
4. **Update Views:** Add permission checks to show/hide UI elements
5. **Test:** Login with different roles and verify access control

---

## Support

For questions or issues:
- Review `ROLES_AND_PERMISSIONS.md` for detailed documentation
- Check `/Account/RolesGuide` for visual permission matrix
- Contact SuperAdmin for role/permission changes

---

**Version:** 1.0  
**Last Updated:** [Current Date]  
**Maintained By:** Development Team
