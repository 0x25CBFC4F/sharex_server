using Microsoft.AspNetCore.Mvc;
using ShareXServer.Database.Enums;
using ShareXServer.Models;
using ShareXServer.Services.Medias;
using ShareXServer.Services.Urls;

namespace ShareXServer.Controllers;

[Controller]
[Route("media")]
public class MediaController : Controller
{
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
        return File(mediaInfo.Stream, mediaInfo.MimeType);
    }
    
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

        var result = await _mediaService.Upload(formFiles.First().OpenReadStream(), isText, cancellationToken);

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