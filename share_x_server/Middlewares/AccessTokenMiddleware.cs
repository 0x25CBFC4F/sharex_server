using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using ShareXServer.Configuration;
using ShareXServer.Models;

namespace ShareXServer.Middlewares;

public class AccessTokenMiddleware
{
    private readonly RequestDelegate _next;

    public AccessTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context, IOptions<ServerOptions> optionsWrapper)
    {
        var options = optionsWrapper.Value;
        var metadata = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata;

        if (metadata == null ||
            !options.AccessToken.Enabled ||
            !metadata.Any(x => x is AccessTokenRequiredAttribute))
        {
            await _next(context);
            return;
        }

        var providedAccessToken = context.Request.Headers.FirstOrDefault(x => x.Key.StartsWith("ShareX-Access-Token"));

        if (providedAccessToken.Equals(default(KeyValuePair<string, StringValues>)))
        {
            await WriteErrorResponse(context, "Access token is not provided!");
            return;
        }

        if (!providedAccessToken.Value.First().Equals(options.AccessToken.Token, StringComparison.InvariantCulture))
        {
            await WriteErrorResponse(context, "Access token is invalid!");
            return;
        }
        
        await _next(context);
    }

    private async Task WriteErrorResponse(HttpContext context, string errorMessage)
    {
        await context.Response.WriteAsJsonAsync(new BaseResponse<object>
        {
            Successful = false,
            ErrorMessage = errorMessage
        });
    }
}