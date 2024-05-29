namespace LoadTests;

public static class Helper
{
    public static readonly string BaseUrl = Environment.GetEnvironmentVariable("BaseUrl") ?? "https://climate-ctrl-staging.azurewebsites.net/api/v1";
}