# ShareX Screenshot Server

[![DockerHub](https://img.shields.io/badge/Docker%20Image-Get%20on%20DockerHub!-success)](https://hub.docker.com/repository/docker/0x25cbfc4f/sharex_server)
![dotnet](https://img.shields.io/badge/Powered%20by-dotnet-success)

## Short description

This is a simple screenshot server made for [ShareX](https://github.com/ShareX/ShareX).

Current version only allows one type of media upload: screenshot. Text, files, etc. may be implemented in future versions.

## Why?
Most of the custom uploaders for ShareX are written in PHP.

PHP is a good language but only if done properly. Scripts made in a 10-minute timespan could be vulnerable.

## How to run

Easiest way to run it is via `docker-compose`. Go to the [`docker_compose_example`](https://github.com/0x25CBFC4F/sharex_server/tree/master/docker_compose_example) and see ready-to-run Docker Compose file!

How to run:
1. Download [`docker-compose.yaml`](https://raw.githubusercontent.com/0x25CBFC4F/sharex_server/master/docker_compose_example/docker-compose.yaml)
2. Place it whenever you want to host your server
3. Run `docker-compose pull`
4. Run `docker-compose up` or `docker-compose up -d` to run in headless mode.
5. Done!

