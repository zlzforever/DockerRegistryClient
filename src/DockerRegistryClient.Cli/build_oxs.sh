#!/usr/bin/env bash
dotnet build --runtime osx-x64 -c Release
dotnet publish --runtime osx-x64 -c Release --self-contained true /p:PublishSingleFile=true
dotnet build --runtime linux-x64 -c Release
dotnet publish --runtime linux-x64 -c Release --self-contained true /p:PublishSingleFile=true
dotnet build --runtime win-x64 -c Release
dotnet publish --runtime win-x64 -c Release --self-contained true /p:PublishSingleFile=true