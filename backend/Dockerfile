FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /backend

# Copy everything
COPY . ./
# Build and publish a release
RUN dotnet publish ./Music.Backend/Music.Backend.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /backend
COPY --from=build /backend/out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Music.Backend.dll"]
