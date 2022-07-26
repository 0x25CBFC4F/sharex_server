﻿namespace ShareXServer.Configuration;

#nullable disable

public class ServerOptions
{
    public AccessTokenOptions AccessToken { get; set; }
    public MediaCleanupOptions MediaCleanup { get; set; }
    public string MediaDirectory { get; set; }
    public string BaseUrl { get; set; }
}