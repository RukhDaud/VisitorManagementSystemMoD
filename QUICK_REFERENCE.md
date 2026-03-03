# 🎯 Quick Reference: Visitor Management System RBAC

## 📋 Role Summary Table

| Role | Symbol | Primary Function | Can Approve? | Can Create? | Can Check-In? | Manage Users? |
|------|--------|-----------------|--------------|-------------|---------------|---------------|
| **SuperAdmin** | 👑 | Full System Control | ✅ | ✅ | ✅ | ✅ |
| **Admin** | 🔐 | System Oversight | ✅ | ✅ | ✅ | ❌ |
| **Security Officer** | 👮 | Approval Management | ✅ | ❌ | ❌ | ❌ |
| **Employee** | 👤 | Personal Visitors | ❌ | ✅ (Own) | ❌ | ❌ |
| **Reception** | 🚪 | Check-In/Out | ❌ | ❌ | ✅ | ❌ |

---

## 🔑 Key Permissions by Role

### 👑 SuperAdmin
```
✅ ALL PERMISSIONS
- Full system access
- User & role management
- All reports
- All visitor operations
```

### 🔐 Admin
```
✅ Visitor Management
✅ Approve/Reject
✅ Check-In/Out
✅ All Reports
❌ User Management
❌ Role Management
```

### 👮 Security Officer
```
✅ View All Requests
✅ Approve/Reject
✅ Generate Reports
❌ Create/Edit Visitors
❌ Check-In/Out
```

### 👤 Employee
```
✅ Create Own Visitors
✅ Edit Own Visitors
✅ View Own Requests
❌ View Others' Requests
❌ Approve/Reject
```

### 🚪 Reception
```
✅ Check-In Visitors
✅ Check-Out Visitors
✅ View Approved Requests
✅ Limited Reports
❌ Create Visitors
❌ Approve/Reject
```

---

## 💻 Quick Code Examples

### Protect Controller Action
```csharp
using VisitorManagementSystemMoD.Attributes;
using VisitorManagementSystemMoD.Constants;

[RequireRole(Roles.SuperAdmin)]
public IActionResult ManageUsers() { }

[RequirePermission(Permissions.ApproveVisitorRequest)]
public IActionResult Approve(int id) { }
```

### Check Permission in View
```razor
@if (Context.HasPermission(Permissions.CreateVisitorRequest))
{
    <a asp-action="Create" class="btn btn-primary">Create Visitor</a>
}
```

### Check Role in Code
```csharp
if (HttpContext.IsInRole(Roles.SuperAdmin, Roles.Admin))
{
    // Admin actions
}
```

---

## 📁 Files Reference

| File | Purpose |
|------|---------|
| `Constants/RolePermissions.cs` | Roles, permissions, mappings |
| `Attributes/AuthorizationAttributes.cs` | Authorization attributes |
| `Services/AuthorizationService.cs` | Permission checking service |
| `Views/Account/AccessDenied.cshtml` | Access denied page |
| `Views/Account/RolesGuide.cshtml` | Interactive permission matrix |
| `ROLES_AND_PERMISSIONS.md` | Complete documentation |
| `IMPLEMENTATION_GUIDE.md` | Implementation instructions |

---

## 🚀 Quick Start

### 1. View Documentation
Open `ROLES_AND_PERMISSIONS.md` for complete details

### 2. Access Interactive Guide
Navigate to: **`/Account/RolesGuide`**

### 3. Apply to Your Code
```csharp
// Add to controller
[RequireRole(Roles.YourRole)]

// Or use permission
[RequirePermission(Permissions.YourPermission)]
```

---

## 🎓 Common Scenarios

### Scenario 1: Employee wants to create visitor
✅ **Allowed** - Has `CreateVisitorRequest` permission

### Scenario 2: Employee wants to approve visitor
❌ **Denied** - Only Security Officer, Admin, SuperAdmin can approve

### Scenario 3: Reception wants to check-in visitor
✅ **Allowed** - Has `CheckInVisitor` permission

### Scenario 4: Security Officer wants to create visitor
❌ **Denied** - Security Officers can only approve/reject

### Scenario 5: SuperAdmin wants to do anything
✅ **Allowed** - SuperAdmin has all permissions

---

## ⚠️ Important Notes

1. **Always use constants** - Don't hard-code role/permission names
2. **Check on server** - Never rely on client-side checks alone
3. **Session-based** - User role stored in session after login
4. **Clear session** - Log out and back in after role changes

---

## 🔍 Debugging Tips

### Check Current User Role
```csharp
var userRole = HttpContext.Session.GetString("UserRole");
Console.WriteLine($"Current Role: {userRole}");
```

### Check Permissions
```csharp
var permissions = HttpContext.GetUserPermissions();
foreach(var p in permissions)
{
    Console.WriteLine(p);
}
```

### Verify Permission
```csharp
var hasPermission = HttpContext.HasPermission(Permissions.YourPermission);
Console.WriteLine($"Has Permission: {hasPermission}");
```

---

## 📞 Getting Help

1. **Read Full Docs:** `ROLES_AND_PERMISSIONS.md`
2. **Implementation Guide:** `IMPLEMENTATION_GUIDE.md`
3. **Visual Reference:** `/Account/RolesGuide`
4. **Contact:** System Administrator or SuperAdmin

---

**Version:** 1.0  
**Status:** ✅ Build Successful  
**Ready to Use:** Yes
