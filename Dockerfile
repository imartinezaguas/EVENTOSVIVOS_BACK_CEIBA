# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos
COPY ["EventosVivos.sln", "./"]
COPY ["EventosVivos.Api/EventosVivos.Api.csproj", "EventosVivos.Api/"]
COPY ["EventosVivos.Application/EventosVivos.Application.csproj", "EventosVivos.Application/"]
COPY ["EventosVivos.Domain/EventosVivos.Domain.csproj", "EventosVivos.Domain/"]
COPY ["EventosVivos.Infrastructure/EventosVivos.Infrastructure.csproj", "EventosVivos.Infrastructure/"]
COPY ["EventosVivos.Tests/EventosVivos.Tests.csproj", "EventosVivos.Tests/"]

# Restaurar dependencias
RUN dotnet restore "EventosVivos.sln"

# Copiar todo el código
COPY . .

# Publicar el proyecto de la API
WORKDIR "/src/EventosVivos.Api"
RUN dotnet publish "EventosVivos.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

# Ejecutar la API
ENTRYPOINT ["dotnet", "EventosVivos.Api.dll"]
