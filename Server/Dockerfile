#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Server/CovidInformationPortal.Server.csproj", "Server/"]
COPY ["Client/CovidInformationPortal.Client.csproj", "Client/"]
COPY ["CovidInformationPortal.Services/CovidInformationPortal.Services.csproj", "CovidInformationPortal.Services/"]
COPY ["CovidInformationPortal.Data/CovidInformationPortal.Data.csproj", "CovidInformationPortal.Data/"]
COPY ["CovidInformationPortal.Models/CovidInformationPortal.Models.csproj", "CovidInformationPortal.Models/"]
RUN dotnet restore "Server/CovidInformationPortal.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "CovidInformationPortal.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CovidInformationPortal.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CovidInformationPortal.Server.dll"]