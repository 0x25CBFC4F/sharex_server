namespace ShareXServer.Database.Models;

/// <summary>
/// Screenshot DB entity.
/// </summary>
public class Screenshot
{
    /// <summary>
    /// Unique GUID.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Upload date in UTC.
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// File name on the disk.
    /// </summary>
    public string FileName { get; set; } = null!;
    
    /// <summary>
    /// Delete token.
    /// </summary>
    public string DeleteToken { get; set; } = null!;
}