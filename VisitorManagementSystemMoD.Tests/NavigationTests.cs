using OpenQA.Selenium;

namespace VisitorManagementSystemMoD.Tests;

public class NavigationTests : BaseTest
{
    [Fact]
    public void UserManagement_RequiresAuthentication()
    {
        NavigateTo("/UserManagement");
        WaitForPageLoad();
        Assert.Contains("/Account/Login", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RoleManagement_RequiresAuthentication()
    {
        NavigateTo("/RoleManagement");
        WaitForPageLoad();
        Assert.Contains("/Account/Login", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DepartmentManagement_RequiresAuthentication()
    {
        NavigateTo("/DepartmentManagement");
        WaitForPageLoad();
        Assert.Contains("/Account/Login", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void VisitorPage_RequiresAuthentication()
    {
        NavigateTo("/Visitor");
        WaitForPageLoad();
        Assert.Contains("/Account/Login", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Administration_RequiresAuthentication()
    {
        NavigateTo("/Dashboard/Administration");
        WaitForPageLoad();
        Assert.Contains("/Account/Login", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserManagement_LoadsForSuperAdmin()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);

        NavigateTo("/UserManagement");
        WaitForPageLoad();

        Assert.Contains("/UserManagement", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RoleManagement_LoadsForSuperAdmin()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);

        NavigateTo("/RoleManagement");
        WaitForPageLoad();

        Assert.Contains("/RoleManagement", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DepartmentManagement_LoadsForSuperAdmin()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);

        NavigateTo("/DepartmentManagement");
        WaitForPageLoad();

        Assert.Contains("/DepartmentManagement", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserManagement_Create_LoadsForSuperAdmin()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);

        NavigateTo("/UserManagement/Create");
        WaitForPageLoad();

        Assert.Contains("/UserManagement/Create", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DashboardAjax_RefreshCounts_ReturnsData()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);

        NavigateTo("/Dashboard/RefreshCounts");
        WaitForPageLoad();

        var pageSource = Driver.PageSource;
        Assert.Contains("checkedIn", pageSource, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("checkedOut", pageSource, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DashboardAjax_GetVisitorStats_ReturnsData()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);

        NavigateTo("/Dashboard/GetVisitorStats");
        WaitForPageLoad();

        var pageSource = Driver.PageSource;
        Assert.Contains("success", pageSource, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DashboardAjax_GenerateReport_ReturnsData()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);

        NavigateTo("/Dashboard/GenerateReport?type=checkedin&period=today");
        WaitForPageLoad();

        var pageSource = Driver.PageSource;
        Assert.Contains("success", pageSource, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void HomePage_Loads()
    {
        NavigateTo("/");
        WaitForPageLoad();

        // Should either show home page or redirect to login
        Assert.False(string.IsNullOrEmpty(Driver.PageSource), "Page should have content");
    }
}
