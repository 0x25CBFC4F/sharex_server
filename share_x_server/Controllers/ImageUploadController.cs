﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ShareXServer.Configuration;
using ShareXServer.Models;
using ShareXServer.Services.Screenshots;
using ShareXServer.Services.Urls;

namespace ShareXServer.Controllers;

[Controller]
[Route("s")]
public class ImageUploadController : Controller
{
    private readonly IOptions<ServerOptions> _serverOptions;
    private readonly IScreenshotService _screenshotService;
    private readonly IUrlGeneratorService _urlGeneratorService;

    public ImageUploadController(IOptions<ServerOptions> serverOptions, IScreenshotService screenshotService, IUrlGeneratorService urlGeneratorService)
    {
        _serverOptions = serverOptions;
        _screenshotService = screenshotService;
        _urlGeneratorService = urlGeneratorService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ViewScreenshot(Guid id, CancellationToken cancellationToken)
    {
        var screenshot = await _screenshotService.GetScreenshot(id, cancellationToken);

        if (screenshot.IsFailed)
        {
            return Json(new BaseResponse<object>
            {
                Successful = false,
                ErrorMessage = "No such screenshot was found."
            });
        }

        return File(screenshot.Value, "image/png");
    }
    
    [HttpPost("u")]
    public async Task<BaseResponse<ScreenshotUploadResult>> UploadScreenshot(CancellationToken cancellationToken)
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

        var result = await _screenshotService.UploadScreenshot(formFiles.First().OpenReadStream(), cancellationToken);

        if (result.IsFailed)
        {
            return new BaseResponse<ScreenshotUploadResult>
            {
                Successful = false,
                ErrorMessage = "Failed to upload a screenshot."
            };
        }

        var (viewUrl, deleteUrl) = _urlGeneratorService.GenerateFor(result.Value);
        
        return new BaseResponse<ScreenshotUploadResult>
        {
            Successful = true,
            Data = new ScreenshotUploadResult
            {
                ScreenshotUrl = viewUrl,
                DeletionUrl = deleteUrl
            }
        };
    }

    [HttpGet("d/{deletionToken}")]
    public async Task<BaseResponse<object>> DeleteScreenshot(string deletionToken, CancellationToken cancellationToken)
    {
        var screenshot = await _screenshotService.DeleteScreenshot(deletionToken, cancellationToken);

        if (screenshot.IsFailed)
        {
            return new BaseResponse<object>
            {
                Successful = false,
                ErrorMessage = screenshot.Errors.First().Message
            };
        }

        return new BaseResponse<object>
        {
            Successful = true
        };
    }
}