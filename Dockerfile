# Utilizar una imagen base de .NET para aplicaciones ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Establecer directorio de trabajo dentro del contenedor
WORKDIR /app

# Exponer los puertos necesario (80 y 443 son puertos estandares)
EXPOSE 80
EXPOSE 443
EXPOSE 5251

# Imagen base para construir la aplicacion
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Establecer el directorio para la etapa de construccion
WORKDIR /src

# Copiar el archivo del proyecto ApiCubeTB.csproj desde la máquina local al directorio de trabajo del contenedor
COPY ["ApiCubeTB.csproj", "./"]

# Restaurar las dependencias y los paquetes NuGet del archivo del proyecto ApiCubeTB.csproj
RUN dotnet restore "ApiCubeTB.csproj"

# Copiar todos los archivos del directorio actual al directorio de trabajo (/src)
COPY . .

# Establecer nuevamente el directorio de trabajo donde están los archivos copiados
WORKDIR "/src/."

# Compilar la aplicación en modo Release y publica los binarios dentro de /app/publish
RUN dotnet publish "ApiCubeTB.csproj" -c Release -o /app/publish

# Reutilizar la imagen base definida al inicio del dockerfile
FROM base AS final

# Definir el directorio de trabajo como '/app'
WORKDIR /app

# Copiar los archivos publicados desde la etapa build al directorio de trabajo actual
COPY --from=build /app/publish .

# Comando para iniciar la aplicacion
ENTRYPOINT ["dotnet", "ApiCubeTB.dll"]
