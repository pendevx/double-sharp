﻿FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /backend

# Copy everything
COPY . ./
# Build and publish a release
RUN dotnet publish ./Music.Backend/Music.Backend.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /backend
COPY --from=build /backend/out .

RUN apt-get update && apt-get install -y curl unzip python3

# yt-dlp
RUN curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o /backend/yt-dlp \
    && chmod +x /backend/yt-dlp

# ffmpeg
RUN curl -L https://github.com/ffbinaries/ffbinaries-prebuilt/releases/download/v6.1/ffmpeg-6.1-linux-64.zip -o /backend/ffmpeg.zip \
    && unzip /backend/ffmpeg.zip -d /backend \
    && rm /backend/ffmpeg.zip

# Restore working directory for .NET app
WORKDIR /backend

EXPOSE 8080
ENTRYPOINT ["dotnet", "Music.Backend.dll"]
