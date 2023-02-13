FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

#install fonts
RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
RUN apt-get update; apt-get install -y ttf-mscorefonts-installer fontconfig

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DauInvestTest.WebApp/DauInvestTest.WebApp.csproj", "DauInvestTest.WebApp/"]
RUN dotnet restore "DauInvestTest.WebApp/DauInvestTest.WebApp.csproj"
COPY . .
WORKDIR "/src/DauInvestTest.WebApp"
RUN dotnet build "DauInvestTest.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DauInvestTest.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DauInvestTest.WebApp.dll"]
