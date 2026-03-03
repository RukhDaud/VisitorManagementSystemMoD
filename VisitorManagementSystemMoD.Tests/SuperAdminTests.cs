using OpenQA.Selenium;

namespace VisitorManagementSystemMoD.Tests;

public class SuperAdminTests : BaseTest
{
    private void NavigateToAdministration()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);
        NavigateTo("/Dashboard/Administration");
        WaitForPageLoad();
    }

    [Fact]
    public void Administration_LoadsSuccessfully()
    {
        NavigateToAdministration();
        Assert.Contains("/Dashboard/Administration", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Administration_DisplaysSystemAdminHeader()
    {
        NavigateToAdministration();

        var pageSource = Driver.PageSource;
        Assert.Contains("System Administration", pageSource);
    }

    [Fact]
    public void Administration_DisplaysQuickStatsCards()
    {
        NavigateToAdministration();

        // Should have 4 stat cards: Total Users, Total Roles, Departments, Total Visitors
        var pageSource = Driver.PageSource;
        Assert.Contains("Total Users", pageSource);
        Assert.Contains("Total Roles", pageSource);
        Assert.Contains("Departments", pageSource);
        Assert.Contains("Total Visitors", pageSource);
    }

    [Fact]
    public void Administration_QuickStatsCards_AreClickable()
    {
        NavigateToAdministration();

        // Check that the stat cards are links
        var statLinks = Driver.FindElements(By.CssSelector("a[href*='UserManagement'], a[href*='RoleManagement'], a[href*='DepartmentManagement'], a[href*='Visitor']"));
        Assert.True(statLinks.Count >= 3, "Quick stat cards should be clickable links");
    }

    [Fact]
    public void Administration_HasCollapsibleRolesSection()
    {
        NavigateToAdministration();

        var rolesSection = ElementExists(By.Id("rolesSection"));
        Assert.True(rolesSection, "Roles section should exist");

        var pageSource = Driver.PageSource;
        Assert.Contains("System Roles", pageSource);
    }

    [Fact]
    public void Administration_HasCollapsibleUsersSection()
    {
        NavigateToAdministration();

        var usersSection = ElementExists(By.Id("usersSection"));
        Assert.True(usersSection, "Users section should exist");

        var pageSource = Driver.PageSource;
        Assert.Contains("System Users", pageSource);
    }

    [Fact]
    public void Administration_HasAlertsSection()
    {
        NavigateToAdministration();

        var alertsSection = ElementExists(By.Id("alertsSection"));
        Assert.True(alertsSection, "Alerts section should exist");

        var pageSource = Driver.PageSource;
        Assert.Contains("System Alerts", pageSource);
    }

    [Fact]
    public void Administration_RolesSection_ExpandsOnClick()
    {
        NavigateToAdministration();

        // Click to expand roles section
        var rolesHeader = Driver.FindElement(By.CssSelector("[onclick*='rolesSection']"));
        rolesHeader.Click();
        Thread.Sleep(500);

        var rolesSection = Driver.FindElement(By.Id("rolesSection"));
        Assert.True(rolesSection.Displayed, "Roles section should be visible after clicking header");
    }

    [Fact]
    public void Administration_UsersSection_ExpandsOnClick()
    {
        NavigateToAdministration();

        var usersHeader = Driver.FindElement(By.CssSelector("[onclick*='usersSection']"));
        usersHeader.Click();
        Thread.Sleep(500);

        var usersSection = Driver.FindElement(By.Id("usersSection"));
        Assert.True(usersSection.Displayed, "Users section should be visible after clicking header");
    }

    [Fact]
    public void Administration_UserSearch_FiltersResults()
    {
        NavigateToAdministration();

        // Expand users section first
        var usersHeader = Driver.FindElement(By.CssSelector("[onclick*='usersSection']"));
        usersHeader.Click();
        Thread.Sleep(500);

        var searchInput = Driver.FindElement(By.Id("userSearch"));
        searchInput.SendKeys("System Administrator");
        Thread.Sleep(300);

        var visibleRows = Driver.FindElements(By.CssSelector(".user-row"))
            .Where(r => r.Displayed)
            .ToList();

        Assert.True(visibleRows.Count >= 1, "Search should show matching users");
    }

    [Fact]
    public void Administration_HasAddUserButton()
    {
        NavigateToAdministration();

        var pageSource = Driver.PageSource;
        Assert.Contains("Add User", pageSource);

        var addUserLink = Driver.FindElements(By.CssSelector("a[href*='UserManagement/Create']"));
        Assert.True(addUserLink.Count > 0, "Add User button should link to UserManagement/Create");
    }

    [Fact]
    public void Administration_DisplaysWelcomeMessage()
    {
        NavigateToAdministration();

        var pageSource = Driver.PageSource;
        Assert.Contains("Welcome back", pageSource);
    }

    [Fact]
    public void Administration_BellButton_ScrollsToAlerts()
    {
        NavigateToAdministration();

        var bellButton = Driver.FindElement(By.CssSelector("button[onclick='scrollToAlerts()']"));
        Assert.True(bellButton.Displayed, "Bell button should be visible");

        bellButton.Click();
        Thread.Sleep(1000);

        // Alerts section should be visible after scrolling
        var alertsSection = Driver.FindElement(By.Id("alerts-section"));
        Assert.True(alertsSection.Displayed, "Alerts section should be visible after clicking bell");
    }
}
