namespace ShareXServer.Configuration;

public class MediaCleanupOptions
{
    public bool Enabled { get; set; }
    public TimeSpan CheckInterval { get; set; }
    public TimeSpan MaxMediaLifespan { get; set; }
}