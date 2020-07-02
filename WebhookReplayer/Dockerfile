FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

RUN apt-get update \
    && apt-get install -y gss-ntlmssp \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "WebhookReplayer.dll"]
