using OpenQA.Selenium;

namespace VisitorManagementSystemMoD.Tests;

public class AlertTests : BaseTest
{
    private void NavigateToAdministration()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);
        NavigateTo("/Dashboard/Administration");
        WaitForPageLoad();
    }

    [Fact]
    public void AlertsApi_GetActiveAlerts_ReturnsSuccess()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);

        NavigateTo("/Alert/GetActiveAlerts");
        WaitForPageLoad();

        var pageSource = Driver.PageSource;
        Assert.Contains("success", pageSource, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AlertsApi_GetAlertCount_ReturnsSuccess()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);

        NavigateTo("/Alert/GetAlertCount");
        WaitForPageLoad();

        var pageSource = Driver.PageSource;
        Assert.Contains("success", pageSource, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AlertsApi_GetAllAlerts_ReturnsSuccess()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);

        NavigateTo("/Alert/GetAllAlerts");
        WaitForPageLoad();

        var pageSource = Driver.PageSource;
        Assert.Contains("success", pageSource, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Alerts_CreateModal_OpensAndCloses()
    {
        NavigateToAdministration();

        // Click "New Alert" button
        var newAlertBtn = Driver.FindElement(By.CssSelector("button[onclick*='openCreateAlertModal']"));
        newAlertBtn.Click();
        Thread.Sleep(500);

        var modal = Driver.FindElement(By.Id("createAlertModal"));
        Assert.True(modal.Displayed, "Create Alert modal should be visible");

        // Verify modal fields exist
        Assert.True(ElementExists(By.Id("alertTitle")), "Alert Title field should exist");
        Assert.True(ElementExists(By.Id("alertMessage")), "Alert Message field should exist");
        Assert.True(ElementExists(By.Id("alertPriority")), "Alert Priority field should exist");
        Assert.True(ElementExists(By.Id("alertCategory")), "Alert Category field should exist");
        Assert.True(ElementExists(By.Id("alertExpiresAt")), "Alert ExpiresAt field should exist");

        // Close modal
        Driver.FindElement(By.CssSelector("#createAlertModal button[onclick*='closeCreateAlertModal']")).Click();
        Thread.Sleep(500);

        Assert.False(modal.Displayed, "Modal should be hidden after closing");
    }

    [Fact]
    public void Alerts_CreateModal_ValidatesRequiredFields()
    {
        NavigateToAdministration();

        // Open modal
        Driver.FindElement(By.CssSelector("button[onclick*='openCreateAlertModal']")).Click();
        Thread.Sleep(500);

        // Click publish without filling fields
        Driver.FindElement(By.Id("submitAlertBtn")).Click();
        Thread.Sleep(500);

        // Should show a browser alert for validation
        try
        {
            var alert = Driver.SwitchTo().Alert();
            Assert.Contains("required", alert.Text, StringComparison.OrdinalIgnoreCase);
            alert.Accept();
        }
        catch (NoAlertPresentException)
        {
            // If no browser alert, modal should still be open (validation prevented submit)
            var modal = Driver.FindElement(By.Id("createAlertModal"));
            Assert.True(modal.Displayed, "Modal should remain open when validation fails");
        }
    }

    [Fact]
    public void Alerts_CreateAndVerify()
    {
        NavigateToAdministration();

        // Open create alert modal
        Driver.FindElement(By.CssSelector("button[onclick*='openCreateAlertModal']")).Click();
        Thread.Sleep(500);

        var uniqueTitle = $"Test Alert {DateTime.Now:HHmmss}";

        // Fill in alert form
        var titleField = Driver.FindElement(By.Id("alertTitle"));
        titleField.Clear();
        titleField.SendKeys(uniqueTitle);

        var messageField = Driver.FindElement(By.Id("alertMessage"));
        messageField.Clear();
        messageField.SendKeys("This is a Selenium automated test alert.");

        // Set priority to Warning
        var prioritySelect = Driver.FindElement(By.Id("alertPriority"));
        prioritySelect.SendKeys("Warning");

        // Set category to System
        var categorySelect = Driver.FindElement(By.Id("alertCategory"));
        categorySelect.SendKeys("System");

        // Submit
        Driver.FindElement(By.Id("submitAlertBtn")).Click();
        Thread.Sleep(2000);

        // Modal should close
        var modal = Driver.FindElement(By.Id("createAlertModal"));
        Assert.False(modal.Displayed, "Modal should close after successful creation");

        // Verify the alert appears in the active alerts or history
        WaitForPageLoad();
        Thread.Sleep(1000);

        var pageSource = Driver.PageSource;
        Assert.Contains(uniqueTitle, pageSource, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Alerts_HistoryTable_LoadsData()
    {
        NavigateToAdministration();
        Thread.Sleep(2000); // Wait for AJAX

        var historyTable = ElementExists(By.Id("alertsHistoryTable"));
        Assert.True(historyTable, "Alerts history table should exist");

        var historyBody = Driver.FindElement(By.Id("alertsHistoryBody"));
        Assert.NotNull(historyBody);
    }

    [Fact]
    public void Alerts_ActiveAlertsList_Loads()
    {
        NavigateToAdministration();
        Thread.Sleep(2000);

        var activeList = ElementExists(By.Id("activeAlertsList"));
        Assert.True(activeList, "Active alerts list should exist");
    }

    [Fact]
    public void AlertsApi_RequiresAuthentication()
    {
        // Try accessing alerts API without login
        NavigateTo("/Alert/GetActiveAlerts");
        WaitForPageLoad();

        // Should redirect to login or return unauthorized
        var url = Driver.Url;
        var pageSource = Driver.PageSource;

        var isRedirectedOrDenied = url.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase)
                                   || pageSource.Contains("\"success\":false", StringComparison.OrdinalIgnoreCase);

        Assert.True(isRedirectedOrDenied, "Alert API should require authentication");
    }
}
