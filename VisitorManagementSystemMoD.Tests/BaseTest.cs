using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace VisitorManagementSystemMoD.Tests;

public abstract class BaseTest : IDisposable
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;
    protected readonly TestSettings Settings;

    protected BaseTest()
    {
        Settings = TestSettings.Load();

        Driver = Settings.Browser.ToLower() switch
        {
            "edge" => CreateEdgeDriver(),
            _ => CreateChromeDriver()
        };

        Driver.Manage().Window.Maximize();
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(Settings.DefaultTimeoutSeconds);
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(Settings.DefaultTimeoutSeconds));
    }

    private IWebDriver CreateChromeDriver()
    {
        new DriverManager().SetUpDriver(new ChromeConfig());
        var options = new ChromeOptions();
        if (Settings.HeadlessMode)
        {
            options.AddArgument("--headless=new");
        }
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--ignore-certificate-errors");
        return new ChromeDriver(options);
    }

    private IWebDriver CreateEdgeDriver()
    {
        new DriverManager().SetUpDriver(new EdgeConfig());
        var options = new EdgeOptions();
        if (Settings.HeadlessMode)
        {
            options.AddArgument("--headless=new");
        }
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--ignore-certificate-errors");
        return new EdgeDriver(options);
    }

    protected string Url(string path) => $"{Settings.BaseUrl.TrimEnd('/')}/{path.TrimStart('/')}";

    protected void NavigateTo(string path) => Driver.Navigate().GoToUrl(Url(path));

    protected void Login(string username, string password)
    {
        NavigateTo("/Account/Login");
        WaitForElement(By.CssSelector("input[name='Username']"));

        var usernameField = Driver.FindElement(By.CssSelector("input[name='Username']"));
        var passwordField = Driver.FindElement(By.CssSelector("input[name='Password']"));

        usernameField.Clear();
        usernameField.SendKeys(username);
        passwordField.Clear();
        passwordField.SendKeys(password);

        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
    }

    protected void LoginAsSuperAdmin()
    {
        Login(Settings.SuperAdmin.Username, Settings.SuperAdmin.Password);
    }

    protected IWebElement WaitForElement(By by, int? timeoutSeconds = null)
    {
        var wait = timeoutSeconds.HasValue
            ? new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds.Value))
            : Wait;
        return wait.Until(d => d.FindElement(by));
    }

    protected bool WaitForUrl(string containsPath, int? timeoutSeconds = null)
    {
        var wait = timeoutSeconds.HasValue
            ? new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds.Value))
            : Wait;
        return wait.Until(d => d.Url.Contains(containsPath, StringComparison.OrdinalIgnoreCase));
    }

    protected bool ElementExists(By by)
    {
        try
        {
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            var elements = Driver.FindElements(by);
            return elements.Count > 0;
        }
        finally
        {
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(Settings.DefaultTimeoutSeconds);
        }
    }

    protected void WaitForPageLoad()
    {
        Wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").ToString() == "complete");
    }

    protected object ExecuteJs(string script, params object[] args)
    {
        return ((IJavaScriptExecutor)Driver).ExecuteScript(script, args);
    }

    public void Dispose()
    {
        Driver?.Quit();
        Driver?.Dispose();
        GC.SuppressFinalize(this);
    }
}
