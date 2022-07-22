@echo off
docker build -t 0x25cbfc4f/sharex_server:1.1 -t 0x25cbfc4f/sharex_server:latest  .
docker push 0x25cbfc4f/sharex_server:latest
pause