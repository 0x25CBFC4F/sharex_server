namespace ShareXServer.Configuration;

public class ServerOptions
{
    public string AccessToken { get; set; }
    public string ScreenshotsDirectory { get; set; }
    public string BaseUrl { get; set; }
    public string? PublicUrl { get; set; }
}