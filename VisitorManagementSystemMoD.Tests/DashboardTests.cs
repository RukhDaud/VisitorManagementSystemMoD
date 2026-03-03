using OpenQA.Selenium;

namespace VisitorManagementSystemMoD.Tests;

public class DashboardTests : BaseTest
{
    [Fact]
    public void Dashboard_LoadsSuccessfully_ForSuperAdmin()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);
        WaitForPageLoad();

        Assert.Contains("/Dashboard", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Dashboard_DisplaysKpiCards()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);
        WaitForPageLoad();

        // KPI cards should be visible for Admin/SuperAdmin roles
        var kpiCards = Driver.FindElements(By.CssSelector(".kpi-card, .card"));
        Assert.True(kpiCards.Count > 0, "Dashboard should display KPI cards");
    }

    [Fact]
    public void Dashboard_DisplaysWelcomeUsername()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);
        WaitForPageLoad();

        var pageSource = Driver.PageSource;
        Assert.Contains("System Administrator", pageSource, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Dashboard_BellIcon_IsVisible()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);
        WaitForPageLoad();

        var bellIcon = ElementExists(By.CssSelector("#alertBellBtn, .fa-bell"));
        Assert.True(bellIcon, "Bell icon should be visible on the dashboard");
    }

    [Fact]
    public void Dashboard_BellIcon_ShowsDropdownOnClick()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);
        WaitForPageLoad();

        // Wait a moment for alert bell JS to initialize
        Thread.Sleep(2000);

        var bellButton = Driver.FindElements(By.Id("alertBellBtn"));
        if (bellButton.Count > 0)
        {
            bellButton[0].Click();
            Thread.Sleep(500);

            var dropdown = ElementExists(By.Id("alertDropdown"));
            Assert.True(dropdown, "Alert dropdown should appear when bell icon is clicked");
        }
    }

    [Fact]
    public void Dashboard_NavigationLinks_ArePresent()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);
        WaitForPageLoad();

        // Check navigation bar has expected links
        var navLinks = Driver.FindElements(By.CssSelector("nav a, .navbar a, .sidebar a"));
        Assert.True(navLinks.Count > 0, "Navigation links should be present");
    }
}
