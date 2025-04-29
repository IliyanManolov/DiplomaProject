# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Frontend/HomeOwners.Web.UI/HomeOwners.Web.UI.csproj", "Frontend/HomeOwners.Web.UI/"]

COPY ["lib/", "lib/"]

RUN dotnet restore "./Frontend/HomeOwners.Web.UI/HomeOwners.Web.UI.csproj"
COPY . .
WORKDIR "/src/Frontend/HomeOwners.Web.UI"
RUN dotnet build "./HomeOwners.Web.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HomeOwners.Web.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeOwners.Web.UI.dll"]