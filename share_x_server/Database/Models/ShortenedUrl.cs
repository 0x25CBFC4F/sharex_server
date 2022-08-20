namespace ShareXServer.Database.Models;

/// <summary>
/// Shortened URL DB entity.
/// </summary>
public class ShortenedUrl
{
    /// <summary>
    /// ID.
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// URL to redirect to.
    /// </summary>
    public string? RealUrl { get; set; }
}