FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Server/Projekt.Server.csproj", "Server/"]
COPY ["Client/Projekt.Client.csproj", "Client/"]
RUN dotnet restore "Server/Projekt.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "Projekt.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Projekt.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ARG ConnectionStrings__StudyContext
ENV ConnectionStrings__StudyContext=${ConnectionStrings__StudyContext}

ENTRYPOINT ["dotnet", "Projekt.Server.dll"]