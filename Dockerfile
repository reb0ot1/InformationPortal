FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

COPY . /usr/src/app
WORKDIR /usr/src/app/Server
RUN dotnet restore 
RUN dotnet build

FROM build AS publish
RUN dotnet publish "CovidInformationPortal.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CovidInformationPortal.Server.dll"]
# ENTRYPOINT sh /usr/src/app/Server/entrypoint.sh
# ENTRYPOINT ["dotnet", "CovidInformationPortal.Server.dll"]