#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["examples/AspNetCoreClient/AspNetCoreClient.csproj", "examples/AspNetCoreClient/"]
COPY ["src/SimCaptcha.AspNetCore/SimCaptcha.AspNetCore.csproj", "src/SimCaptcha.AspNetCore/"]
COPY ["src/SimCaptcha/SimCaptcha.csproj", "src/SimCaptcha/"]
RUN dotnet restore "examples/AspNetCoreClient/AspNetCoreClient.csproj"
COPY . .
WORKDIR "/src/examples/AspNetCoreClient"
RUN dotnet build "AspNetCoreClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AspNetCoreClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspNetCoreClient.dll"]