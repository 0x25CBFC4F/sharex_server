# How to configure and run ShareX Server

## Docker-compose

Easiest way to run it is via `docker-compose`. Go to the [`docker-compose-example`](https://github.com/0x25CBFC4F/sharex_server/tree/master/docker-compose-example) and see ready-to-run Docker Compose file!

How to run:
1. Download [`docker-compose.yaml`](https://github.com/0x25CBFC4F/sharex_server/blob/master/docker-compose-example/docker-compose.yaml) and [`appsettings.Production.json`](https://github.com/0x25CBFC4F/sharex_server/blob/master/docker-compose-example/appsettings.Production.json)
2. Change desired settings in `appsettings.Production.json`
3. Place it whenever you want to host your server
4. Run `docker-compose pull`
5. Run `docker-compose up` or `docker-compose up -d` to run in headless mode.
6. Done!

## Docker images

You can find Docker images at [hub.docker.com](https://hub.docker.com/repository/registry-1.docker.io/0x25cbfc4f/sharex_server/general).

Tag table:

| Tag | Stable | Description |
| --- | ------ | ----------- |
| latest | ❌ | Latest image built by Jenkins |
| stable | ✔️ | Latest stable version |
| 1.1.### | ❌ | Image built by Jenkins, where ### is a build number |

# `appsettings.Production.json` example

```json
{
  "ConnectionStrings": {
    "Default": "User ID=admin;Password=admin;Host=sharex_db;Port=5432;Database=sharex_db;"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore.Infrastructure": "Warning",
        "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
      }
    }
  },
  "AllowedHosts": "*",
  "ShareX": {
    "AccessToken": {
      "Enabled": true,
      "Token": "AAA-BBB-CCC-DDD-EEE-12345"
    },
    "MediaCleanup": {
      "Enabled": true,
      "CheckInterval": "00:04:00",
      "MaxMediaLifespan": "2.00:00:00"
    },
    "MediaDirectory": "./media/",
    "BaseUrl": "http://localhost:7000/"
  }
}

```

Each property will be described in JSONPath style.

# `$.ShareX` - is a root configuration property

`$.ShareX.MediaDirectory` - is a path to where server should save your media files.

`$.ShareX.BaseUrl` - no server can resolve it's outside URL due to containerization/reverse-proxying. This property allows you to set base url.

For example, your ShareX server is hosted at `http://sharex.domain.com/`

And you set `BaseUrl` to `http://sharex.domain.com/`

Then ShareX server will return media URL as: `http://sharex.domain.com/media/XXXXXXXXXXX`

It should work for reverse-proxying too. If your server is hosted at `http://domain.com/sharex`

And you set `BaseUrl` to `http://domain.com/sharex/`

Then ShareX should return media URL as `http://domain.com/sharex/media/XXXXXXXXXXX`

# `$.ShareX.AccessToken` is a section related to authentication.

`$.ShareX.AccessToken.Enabled` - enables access token based security. If set to `false` - server will allow anyone to upload media and shorten links. Does not affect viewing media/links.

`$.ShareX.AccessToken.Token` - your access token. Can be set to anything.

# `$.ShareX.MediaCleanup` is a section related to automatic media file cleanup.

`$.ShareX.MediaCleanup.Enabled` - enables automatic media cleanup to keep free space on the disk.

`$.ShareX.MediaCleanup.CheckInterval` - sets interval in which server will check for outdated media. Format is [`DAYS.HOURS:MINUTES:SECONDS`](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings#the-constant-c-format-specifier). `"04:00:00"` means "every 4 hours"

`$.ShareX.MediaCleanup.MaxMediaLifespan` - sets maximum lifespan of a media. If that time runs out - media gets deleted on the next check. Same format as above.

