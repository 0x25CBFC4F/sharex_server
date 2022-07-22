FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY . /build
WORKDIR /build
RUN dotnet clean
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as final
WORKDIR /app
COPY --from=0 /build/bin/Release/net6.0 /app
ENTRYPOINT dotnet share_x_server.dll
