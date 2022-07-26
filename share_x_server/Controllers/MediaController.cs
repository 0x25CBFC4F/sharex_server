﻿using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using ShareXServer.Database.Enums;
using ShareXServer.Middlewares;
using ShareXServer.Models;
using ShareXServer.Services.Medias;
using ShareXServer.Services.UrlGenerator;

namespace ShareXServer.Controllers;

[Controller]
[Route("media")]
public class MediaController : Controller
{
    private static readonly Regex InvalidFileNameCharactersRegex = new($"[{string.Join("", Path.GetInvalidPathChars().Concat(Path.GetInvalidFileNameChars()).Distinct())}]");
    private readonly IMediaService _mediaService;
    private readonly IUrlGeneratorService _urlGeneratorService;

    public MediaController(IMediaService mediaService, IUrlGeneratorService urlGeneratorService)
    {
        _mediaService = mediaService;
        _urlGeneratorService = urlGeneratorService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> View(Guid id, CancellationToken cancellationToken)
    {
        var mediaResult = await _mediaService.Get(id, cancellationToken);

        if (mediaResult.IsFailed)
        {
            return Json(new BaseResponse<object>
            {
                Successful = false,
                ErrorMessage = "No such media was found."
            });
        }

        var mediaInfo = mediaResult.Value;

        return mediaInfo.MediaType != MediaType.File ?
            File(mediaInfo.Stream, mediaInfo.MimeType, false) :
            File(mediaInfo.Stream, mediaInfo.MimeType, mediaInfo.OriginalFileName);
    }

    [AccessTokenRequired]
    [HttpPost("upload")]
    public async Task<BaseResponse<ScreenshotUploadResult>> Upload([FromQuery] bool isText, CancellationToken cancellationToken)
    {
        var formFiles = Request.Form.Files;

        if (formFiles.Count != 1)
        {
            return new BaseResponse<ScreenshotUploadResult>
            {
                Successful = false,
                ErrorMessage = "Uploaded file count is not 1."
            };
        }

        var formFile = formFiles.First();
        var sanitizedName = InvalidFileNameCharactersRegex.Replace(formFile.FileName.Trim(), "_");
        var result = await _mediaService.Upload(sanitizedName, formFile.OpenReadStream(), isText, cancellationToken);

        if (result.IsFailed)
        {
            return new BaseResponse<ScreenshotUploadResult>
            {
                Successful = false,
                ErrorMessage = "Failed to upload media."
            };
        }

        var (viewUrl, deleteUrl) = _urlGeneratorService.GenerateFor(result.Value);

        return new BaseResponse<ScreenshotUploadResult>
        {
            Successful = true,
            Data = new ScreenshotUploadResult
            {
                Url = viewUrl,
                DeletionUrl = deleteUrl
            }
        };
    }

    [HttpGet("delete/{deletionToken}")]
    public async Task<BaseResponse<object>> Delete(string deletionToken, CancellationToken cancellationToken)
    {
        var media = await _mediaService.Delete(deletionToken, cancellationToken);

        if (media.IsFailed)
        {
            return new BaseResponse<object>
            {
                Successful = false,
                ErrorMessage = media.Errors.First().Message
            };
        }

        return new BaseResponse<object>
        {
            Successful = true
        };
    }
}