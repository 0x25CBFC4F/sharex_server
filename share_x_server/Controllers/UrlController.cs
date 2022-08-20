using FluentResults;
using Microsoft.AspNetCore.Mvc;
using ShareXServer.Database.Models;
using ShareXServer.Middlewares;
using ShareXServer.Models;
using ShareXServer.Services.UrlGenerator;
using ShareXServer.Services.UrlShortener;

namespace ShareXServer.Controllers;

[Controller]
[Route("url")]
public class UrlController : Controller
{
    private readonly IUrlShortenerService _shortenerService;
    private readonly IUrlGeneratorService _urlGeneratorService;

    public UrlController(IUrlShortenerService shortenerService, IUrlGeneratorService urlGeneratorService)
    {
        _shortenerService = shortenerService;
        _urlGeneratorService = urlGeneratorService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Redirect(string id, CancellationToken cancellationToken)
    {
        var url = await _shortenerService.GetRedirectUrl(id, cancellationToken);
        return url.IsSuccess ? Redirect(url.Value!) : Json(Result.Fail(url.Errors));
    }

    [AccessTokenRequired]
    [HttpPost("shorten")]
    public async Task<BaseResponse<ShortenedUrl>> ShortenUrl([FromForm] string realUrl, CancellationToken cancellationToken)
    {
        var shortenedUrl = await _shortenerService.ShortenUrl(realUrl, cancellationToken);
        
        if (!shortenedUrl.IsSuccess)
        {
            return new BaseResponse<ShortenedUrl>
            {
                Successful = false,
                ErrorMessage = shortenedUrl.Errors.First().Message
            };
        }

        var url = _urlGeneratorService.GenerateFor(shortenedUrl.Value);

        return new BaseResponse<ShortenedUrl>
        {
            Successful = true,
            Data = new ShortenedUrl
            {
                RealUrl = url
            }
        };
    }
}