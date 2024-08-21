#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER root
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
USER root #Так делать не стоит TODO
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TestConsoleApplication.csproj", "."]
RUN dotnet restore "./TestConsoleApplication.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./TestConsoleApplication.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TestConsoleApplication.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestConsoleApplication.dll"]