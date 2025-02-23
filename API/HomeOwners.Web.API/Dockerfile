# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/HomeOwners.Web.API/HomeOwners.Web.API.csproj", "API/HomeOwners.Web.API/"]

COPY ["lib/", "lib/"]
COPY ["API/", "API/"]

RUN dotnet restore "./API/HomeOwners.Web.API/HomeOwners.Web.API.csproj"
COPY . .
WORKDIR "/src/API/HomeOwners.Web.API"
RUN dotnet build "./HomeOwners.Web.API.csproj" -c Release -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HomeOwners.Web.API.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeOwners.Web.API.dll"]