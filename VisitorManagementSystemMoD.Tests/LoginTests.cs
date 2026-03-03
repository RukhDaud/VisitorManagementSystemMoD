using OpenQA.Selenium;

namespace VisitorManagementSystemMoD.Tests;

public class LoginTests : BaseTest
{
    [Fact]
    public void LoginPage_Loads_Successfully()
    {
        NavigateTo("/Account/Login");
        WaitForPageLoad();

        Assert.Contains("Login", Driver.Title, StringComparison.OrdinalIgnoreCase);

        var usernameField = Driver.FindElement(By.CssSelector("input[name='Username']"));
        var passwordField = Driver.FindElement(By.CssSelector("input[name='Password']"));
        var submitButton = Driver.FindElement(By.CssSelector("button[type='submit']"));

        Assert.True(usernameField.Displayed);
        Assert.True(passwordField.Displayed);
        Assert.True(submitButton.Displayed);
    }

    [Fact]
    public void Login_WithValidSuperAdmin_RedirectsToDashboard()
    {
        LoginAsSuperAdmin();
        WaitForPageLoad();

        WaitForUrl("/Dashboard", 15);
        Assert.Contains("/Dashboard", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Login_WithInvalidCredentials_ShowsError()
    {
        Login("invaliduser", "wrongpassword");
        WaitForPageLoad();

        Assert.Contains("/Account/Login", Driver.Url, StringComparison.OrdinalIgnoreCase);

        var errorVisible = ElementExists(By.CssSelector(".text-red-500, .text-red-600, .validation-summary-errors"));
        Assert.True(errorVisible, "Error message should be displayed for invalid credentials");
    }

    [Fact]
    public void Login_WithEmptyFields_StaysOnLoginPage()
    {
        NavigateTo("/Account/Login");
        WaitForPageLoad();

        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        WaitForPageLoad();

        Assert.Contains("/Account/Login", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Logout_RedirectsToLoginPage()
    {
        LoginAsSuperAdmin();
        WaitForUrl("/Dashboard", 15);

        NavigateTo("/Account/Logout");
        WaitForPageLoad();

        Assert.Contains("/Account/Login", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Dashboard_WithoutLogin_RedirectsToLogin()
    {
        NavigateTo("/Dashboard");
        WaitForPageLoad();

        Assert.Contains("/Account/Login", Driver.Url, StringComparison.OrdinalIgnoreCase);
    }
}
