# Visitor Management System - Roles & Permissions Documentation

## Overview
This document outlines the role-based access control (RBAC) system implemented in the Visitor Management System. Each role has specific permissions that determine what actions users can perform within the system.

---

## Role Hierarchy

```
SuperAdmin (Highest Authority)
    ↓
Admin
    ↓
Security Officer
    ↓
Employee & Reception
```

---

## Roles and Responsibilities

### 1. SuperAdmin 👑

**Description:** Full system access with complete control over all aspects of the Visitor Management System.

**Primary Responsibilities:**
- Oversee all system operations
- Manage user accounts and role assignments
- Configure system settings and permissions
- Generate comprehensive reports
- Handle all visitor-related operations

**Permissions:**

| Category | Permission | Description |
|----------|-----------|-------------|
| **Visitor Requests** | View All Visitor Requests | Can view all visitor requests across the system |
| | Create Visitor Request | Can create new visitor requests |
| | Edit Any Visitor Request | Can modify any visitor request in the system |
| | Delete Any Visitor Request | Can delete any visitor request |
| | Edit Personal Visitor Request | Can edit own visitor requests |
| | Delete Personal Visitor Request | Can delete own visitor requests |
| **Approvals** | Approve Visitor Request | Can approve pending visitor requests |
| | Reject Visitor Request | Can reject visitor requests |
| **Check-In/Out** | Check-In Visitor | Can check in visitors |
| | Check-Out Visitor | Can check out visitors |
| | View Check-In Status | Can view current check-in status |
| **Reports** | Generate All Reports | Can generate any type of report |
| | View Checked-In Visitors | Can view list of checked-in visitors |
| | View Checked-Out Visitors | Can view list of checked-out visitors |
| | View Pending Visitors | Can view pending visitor requests |
| **User Management** | Create User | Can create new user accounts |
| | Edit User | Can modify user accounts |
| | Delete User | Can delete user accounts |
| | View All Users | Can view all system users |
| **Role Management** | Create Role | Can create new roles |
| | Edit Role | Can modify roles |
| | Delete Role | Can delete roles |
| | View All Roles | Can view all roles |
| | Assign Roles | Can assign roles to users |
| **Department Management** | Create Department | Can create departments |
| | Edit Department | Can modify departments |
| | Delete Department | Can delete departments |
| | View All Departments | Can view all departments |
| **System** | Access Admin Panel | Can access administrative dashboard |
| | Manage System Settings | Can configure system settings |
| | View System Logs | Can view system activity logs |

**Access Level:** ✅ Full Access to All Features

---

### 2. Admin 🔐

**Description:** System oversight and analytics with broad access to visitor management operations but limited system configuration capabilities.

**Primary Responsibilities:**
- Monitor system-wide visitor activities
- Approve/reject visitor requests
- Generate comprehensive reports
- Manage visitor operations
- View user information (read-only)

**Permissions:**

| Category | Permission | ✅/❌ |
|----------|-----------|------|
| **Visitor Requests** | View All Visitor Requests | ✅ |
| | Create Visitor Request | ✅ |
| | Edit Any Visitor Request | ✅ |
| | Delete Any Visitor Request | ❌ |
| | Edit Personal Visitor Request | ✅ |
| | Delete Personal Visitor Request | ✅ |
| **Approvals** | Approve Visitor Request | ✅ |
| | Reject Visitor Request | ✅ |
| **Check-In/Out** | Check-In Visitor | ✅ |
| | Check-Out Visitor | ✅ |
| | View Check-In Status | ✅ |
| **Reports** | Generate All Reports | ✅ |
| | View Checked-In Visitors | ✅ |
| | View Checked-Out Visitors | ✅ |
| | View Pending Visitors | ✅ |
| **User Management** | View All Users | ✅ |
| | Create/Edit/Delete User | ❌ |
| **Role Management** | All Permissions | ❌ |
| **Department Management** | View All Departments | ✅ |
| | Create/Edit/Delete Department | ❌ |
| **System** | Access Admin Panel | ✅ |
| | Manage System Settings | ❌ |

**Access Level:** 🟢 High Access (No Role/User Management)

---

### 3. Security Officer 👮

**Description:** Security Chief Officer responsible for approval management and security oversight.

**Primary Responsibilities:**
- Review and approve/reject visitor requests
- Monitor visitor activities
- Generate visitor-related reports
- View all pending and active visitors
- Ensure security compliance

**Permissions:**

| Category | Permission | ✅/❌ |
|----------|-----------|------|
| **Visitor Requests** | View All Visitor Requests | ✅ |
| | View Personal Visitor Requests | ✅ |
| | Create/Edit/Delete Visitor Request | ❌ |
| **Approvals** | Approve Visitor Request | ✅ |
| | Reject Visitor Request | ✅ |
| **Check-In/Out** | Check-In Visitor | ❌ |
| | Check-Out Visitor | ❌ |
| | View Check-In Status | ✅ |
| **Reports** | Generate Visitor Reports | ✅ |
| | View Checked-In Visitors | ✅ |
| | View Checked-Out Visitors | ✅ |
| | View Pending Visitors | ✅ |
| **User/Role/Dept Management** | All Permissions | ❌ |
| **System** | Access Admin Panel | ❌ |

**Access Level:** 🟡 Medium Access (Approval & Reporting)

**Key Restrictions:**
- ❌ Cannot create, edit, or delete visitor requests
- ❌ Cannot check in/out visitors
- ❌ Cannot manage users, roles, or departments
- ❌ Cannot access admin panel

---

### 4. Employee 👤

**Description:** Standard user who manages their own visitor requests.

**Primary Responsibilities:**
- Create visitor requests for their visitors
- View status of their own requests
- Edit/update their pending visitor requests
- Delete their own visitor requests
- Track their visitors' check-in status

**Permissions:**

| Category | Permission | ✅/❌ |
|----------|-----------|------|
| **Visitor Requests** | View Personal Visitor Requests Only | ✅ |
| | Create Visitor Request | ✅ |
| | Edit Personal Visitor Request | ✅ |
| | Delete Personal Visitor Request | ✅ |
| | View All Visitor Requests | ❌ |
| | Edit/Delete Other's Requests | ❌ |
| **Approvals** | Approve/Reject Requests | ❌ |
| **Check-In/Out** | Check-In Visitor | ❌ |
| | Check-Out Visitor | ❌ |
| | View Check-In Status (Own Visitors) | ✅ |
| **Reports** | Generate Reports | ❌ |
| **Management** | User/Role/Dept Management | ❌ |
| **System** | Access Admin Panel | ❌ |

**Access Level:** 🔵 Limited Access (Personal Requests Only)

**Key Restrictions:**
- ❌ Can only see their own visitor requests
- ❌ Cannot approve or reject any requests
- ❌ Cannot check in/out visitors
- ❌ Cannot generate reports
- ❌ Cannot access other users' data

---

### 5. Reception (Receptionist) 🚪

**Description:** Gate/Reception personnel responsible for visitor check-in and check-out operations.

**Primary Responsibilities:**
- Check in visitors upon arrival
- Check out visitors when they leave
- View approved and pending visitor requests
- Generate check-in/check-out reports
- Manage visitor entry/exit logs

**Permissions:**

| Category | Permission | ✅/❌ |
|----------|-----------|------|
| **Visitor Requests** | View All Visitor Requests | ✅ |
| | View Pending Visitors | ✅ |
| | Create/Edit/Delete Visitor Request | ❌ |
| **Approvals** | Approve/Reject Requests | ❌ |
| **Check-In/Out** | Check-In Visitor | ✅ |
| | Check-Out Visitor | ✅ |
| | View Check-In Status | ✅ |
| **Reports** | Generate Limited Reports | ✅ |
| | View Checked-In Visitors | ✅ |
| | View Checked-Out Visitors | ✅ |
| | Generate All Reports | ❌ |
| **Management** | User/Role/Dept Management | ❌ |
| **System** | Access Admin Panel | ❌ |

**Access Level:** 🟡 Medium Access (Check-In/Out Operations)

**Key Restrictions:**
- ❌ Cannot create or modify visitor requests
- ❌ Cannot approve or reject requests
- ❌ Can only generate check-in/check-out related reports
- ❌ Cannot manage users, roles, or departments

---

## Permission Matrix

| Feature | SuperAdmin | Admin | Security Officer | Employee | Reception |
|---------|-----------|-------|------------------|----------|-----------|
| **View All Requests** | ✅ | ✅ | ✅ | ❌ (Personal Only) | ✅ |
| **Create Visitor** | ✅ | ✅ | ❌ | ✅ | ❌ |
| **Approve/Reject** | ✅ | ✅ | ✅ | ❌ | ❌ |
| **Edit Visitor Info** | ✅ (All) | ✅ (All) | ❌ | ✅ (Own) | ❌ |
| **Delete Visitor** | ✅ (All) | ❌ | ❌ | ✅ (Own) | ❌ |
| **Check-In/Check-Out** | ✅ | ✅ | ❌ | ❌ | ✅ |
| **Generate Reports** | ✅ (All) | ✅ (All) | ✅ (Visitor) | ❌ | ✅ (Limited) |
| **Manage Users** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Manage Roles** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Manage Departments** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **System Settings** | ✅ | ❌ | ❌ | ❌ | ❌ |

---

## Implementation Details

### 1. Code Structure

```
VisitorManagementSystemMoD/
├── Constants/
│   └── RolePermissions.cs          # Defines roles and permissions
├── Attributes/
│   └── AuthorizationAttributes.cs  # Custom authorization attributes
├── Services/
│   └── AuthorizationService.cs     # Permission checking service
└── Models/
    ├── Role.cs                     # Role entity
    └── User.cs                     # User entity
```

### 2. Usage Examples

#### In Controllers (Using Attributes):

```csharp
// Restrict access to specific roles
[RequireRole(Roles.SuperAdmin, Roles.Admin)]
public IActionResult ManageUsers()
{
    // Only SuperAdmin and Admin can access
}

// Restrict access based on permissions
[RequirePermission(Permissions.ApproveVisitorRequest)]
public IActionResult ApproveVisitor(int id)
{
    // Only roles with approval permission can access
}

// Multiple permissions (user needs ANY of them)
[RequirePermission(
    Permissions.EditAnyVisitorRequest, 
    Permissions.EditPersonalVisitorRequest)]
public IActionResult EditVisitor(int id)
{
    // User needs at least one of these permissions
}
```

#### In Views (Using Conditionals):

```razor
@inject IAuthorizationService AuthService

@{
    var userRole = Context.Session.GetString("UserRole");
}

@if (AuthService.HasPermission(userRole, Permissions.CreateVisitorRequest))
{
    <a asp-action="Create" class="btn btn-primary">Create Visitor</a>
}

@if (Context.IsInRole(Roles.SuperAdmin, Roles.Admin))
{
    <div class="admin-panel">
        <!-- Admin content -->
    </div>
}
```

#### Programmatic Permission Checking:

```csharp
// Inject the service
private readonly IAuthorizationService _authService;

// Check single permission
if (_authService.HasPermission(userRole, Permissions.ApproveVisitorRequest))
{
    // Allow approval
}

// Check multiple permissions (ANY)
if (_authService.HasAnyPermission(userRole, 
    Permissions.EditAnyVisitorRequest, 
    Permissions.EditPersonalVisitorRequest))
{
    // Allow edit
}

// Check multiple permissions (ALL)
if (_authService.HasAllPermissions(userRole, 
    Permissions.CreateUser, 
    Permissions.AssignRoles))
{
    // Allow user creation with role assignment
}
```

### 3. HttpContext Extension Methods:

```csharp
// In any controller or view with HttpContext access
if (HttpContext.HasPermission(Permissions.ApproveVisitorRequest))
{
    // User has approval permission
}

if (HttpContext.IsInRole(Roles.SuperAdmin, Roles.Admin))
{
    // User is SuperAdmin or Admin
}

var userPermissions = HttpContext.GetUserPermissions();
// Returns list of all permissions for current user
```

---

## Best Practices

### 1. **Always Check Permissions**
- ✅ Use `[RequirePermission]` attribute on controller actions
- ✅ Check permissions in views before showing UI elements
- ✅ Validate permissions in service layer for critical operations

### 2. **Principle of Least Privilege**
- ✅ Grant only necessary permissions
- ✅ Review role permissions regularly
- ✅ Remove unused permissions

### 3. **Security**
- ✅ Never trust client-side permission checks alone
- ✅ Always validate on server-side
- ✅ Log permission denials for security auditing

### 4. **Consistency**
- ✅ Use constants from `Roles` and `Permissions` classes
- ✅ Don't hard-code role or permission names
- ✅ Maintain documentation when adding new permissions

---

## Adding New Permissions

### Step 1: Define Permission Constant
```csharp
// In Constants/RolePermissions.cs
public static class Permissions
{
    public const string YourNewPermission = "YourNewPermission";
}
```

### Step 2: Add to Role Mapping
```csharp
// In RolePermissionsMapping
{
    Roles.SuperAdmin, new List<string>
    {
        // ... existing permissions
        Permissions.YourNewPermission
    }
}
```

### Step 3: Use in Code
```csharp
[RequirePermission(Permissions.YourNewPermission)]
public IActionResult YourAction()
{
    // Protected action
}
```

---

## Troubleshooting

### Issue: User Can't Access Feature
1. Check if user role is correctly set in session
2. Verify permission is mapped to user's role in `RolePermissionsMapping`
3. Ensure `[RequirePermission]` attribute has correct permission name

### Issue: Permission Not Working
1. Check spelling of permission constant
2. Verify permission is added to role in `RolePermissionsMapping`
3. Clear session and log in again

---

## Future Enhancements

1. **Database-Driven Permissions**
   - Move from hard-coded to database-stored permissions
   - Allow runtime permission updates

2. **Permission Groups**
   - Create permission groups for easier management
   - Example: "Visitor Management" group

3. **Audit Logging**
   - Log all permission checks
   - Track permission changes

4. **Custom Permissions per User**
   - Allow individual permission overrides
   - User-specific permission additions/removals

---

## Support

For questions or issues related to roles and permissions, contact:
- **System Administrator**: SuperAdmin
- **Documentation**: This file
- **Code Reference**: `Constants/RolePermissions.cs`

---

**Last Updated:** [Current Date]  
**Version:** 1.0  
**Maintained By:** Development Team
