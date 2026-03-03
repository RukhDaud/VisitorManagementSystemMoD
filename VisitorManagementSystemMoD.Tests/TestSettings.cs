using System.Text.Json;

namespace VisitorManagementSystemMoD.Tests;

public class TestSettings
{
    public string BaseUrl { get; set; } = "https://localhost:7001";
    public string Browser { get; set; } = "Chrome";
    public bool HeadlessMode { get; set; } = false;
    //true is WeakReference want no browser window
    public int DefaultTimeoutSeconds { get; set; } = 10;
    public TestCredentials SuperAdmin { get; set; } = new();

    public static TestSettings Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.test.json");
        if (!File.Exists(path))
        {
            return new TestSettings();
        }

        var json = File.ReadAllText(path);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement.GetProperty("TestSettings");

        var settings = new TestSettings();

        if (root.TryGetProperty("BaseUrl", out var baseUrl))
            settings.BaseUrl = baseUrl.GetString()!;
        if (root.TryGetProperty("Browser", out var browser))
            settings.Browser = browser.GetString()!;
        if (root.TryGetProperty("HeadlessMode", out var headless))
            settings.HeadlessMode = headless.GetBoolean();
        if (root.TryGetProperty("DefaultTimeoutSeconds", out var timeout))
            settings.DefaultTimeoutSeconds = timeout.GetInt32();

        if (root.TryGetProperty("SuperAdmin", out var sa))
        {
            settings.SuperAdmin = new TestCredentials
            {
                Username = sa.TryGetProperty("Username", out var u) ? u.GetString()! : "superadmin",
                Password = sa.TryGetProperty("Password", out var p) ? p.GetString()! : "super123"
            };
        }

        return settings;
    }
}

public class TestCredentials
{
    public string Username { get; set; } = "superadmin";
    public string Password { get; set; } = "super123";
}
