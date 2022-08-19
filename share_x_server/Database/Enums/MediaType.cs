namespace ShareXServer.Database.Enums;

/// <summary>
/// Media types
/// </summary>
public enum MediaType
{
    /// <summary>
    /// Image media type (PNG, JPEG, GIF, BMP, TIFF)
    /// </summary>
    Image = 0,
    
    /// <summary>
    /// GIF media type (GIF)
    /// </summary>
    Gif,
    
    /// <summary>
    /// Video media type (MP4)
    /// </summary>
    Video,
    
    /// <summary>
    /// Raw file media type
    /// </summary>
    File,
    
    /// <summary>
    /// Plain-text media type
    /// </summary>
    Text
}
