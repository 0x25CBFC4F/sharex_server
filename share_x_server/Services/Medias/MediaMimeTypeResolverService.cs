using System.Diagnostics.CodeAnalysis;
using ShareXServer.Database.Enums;

namespace ShareXServer.Services.Medias;

/// <inheritdoc />
public class MediaMimeTypeResolverService : IMediaMimeTypeResolverService
{
    private static readonly byte[][] GifMagicSignatures =
    {
        new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39 },
        new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37 }
    };

    private static readonly byte[] Mp4Signature = { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D };

    private static readonly byte[] PngMagicSignatures = { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };

    /// <inheritdoc />
    public (MediaType mediaType, string MimeType) Resolve(Stream stream, bool isText)
    {
        if (isText)
        {
            return (MediaType.Text, "text/plain");
        }

        var headerSample = SampleHeader(stream, 64);

        if (ValidateMagicHeader(headerSample, PngMagicSignatures))
        {
            return (MediaType.Image, "image/png");
        }

        if (ValidateMagicHeader(headerSample, GifMagicSignatures))
        {
            return (MediaType.Gif, "image/gif");
        }

        if (ValidateMagicHeader(headerSample, Mp4Signature))
        {
            return (MediaType.Video, "video/mp4");
        }

        return (MediaType.File, "application/octet-stream");
    }

    private static ReadOnlySpan<byte> SampleHeader(Stream stream, int sampleLength)
    {
        stream.Seek(0, SeekOrigin.Begin);
        var headerSample = new byte[Math.Min(stream.Length, sampleLength)];
        _ = stream.Read(headerSample, 0, headerSample.Length);
        stream.Seek(0, SeekOrigin.Begin);

        return new ReadOnlySpan<byte>(headerSample);
    }

    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery", Justification = "Can't use LINQ due to byte-ref structure")]
    private static bool ValidateMagicHeader(ReadOnlySpan<byte> headerSample, byte[][] magicHeaders)
    {
        foreach (var magicHeader in magicHeaders)
        {
            if (ValidateMagicHeader(headerSample, magicHeader))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ValidateMagicHeader(ReadOnlySpan<byte> headerSample, byte[] magicHeader)
    {
        return headerSample.StartsWith(magicHeader);
    }
}