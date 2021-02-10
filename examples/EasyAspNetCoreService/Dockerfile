#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
# 微软 base镜像 存在中文无法显示（方框）问题，尽管是加载自己本地中文字体，依然有此问题，因此用下方自己做的镜像
FROM yiyungent/aspnetcore-runtime-3.1 AS base
# 解决 Linux 下缺少 'libgdiplus'
RUN apt-get update
RUN apt-get install -y --no-install-recommends libgdiplus libc6-dev 
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["examples/EasyAspNetCoreService/EasyAspNetCoreService.csproj", "examples/EasyAspNetCoreService/"]
COPY ["src/SimCaptcha.AspNetCore/SimCaptcha.AspNetCore.csproj", "src/SimCaptcha.AspNetCore/"]
COPY ["src/SimCaptcha/SimCaptcha.csproj", "src/SimCaptcha/"]
RUN dotnet restore "examples/EasyAspNetCoreService/EasyAspNetCoreService.csproj"
COPY . .
WORKDIR "/src/examples/EasyAspNetCoreService"
RUN dotnet build "EasyAspNetCoreService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyAspNetCoreService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyAspNetCoreService.dll"]