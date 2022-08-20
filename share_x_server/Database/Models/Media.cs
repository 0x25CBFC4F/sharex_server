using ShareXServer.Database.Enums;

namespace ShareXServer.Database.Models;

/// <summary>
/// Screenshot DB entity.
/// </summary>
public class Media
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
    /// Media type.
    /// </summary>
    public MediaType MediaType { get; set; }

    /// <summary>
    /// Sanitized original file name.
    /// </summary>
    public string OriginalFileName { get; set; } = null!;
    
    /// <summary>
    /// File name on the disk.
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// File Mime-Type.
    /// </summary>
    public string MimeType { get; set; } = null!;
    
    /// <summary>
    /// Delete token.
    /// </summary>
    public string DeleteToken { get; set; } = null!;
}