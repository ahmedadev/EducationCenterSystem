FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/EducationCenterSystem.Api/EducationCenterSystem.Api.csproj", "src/EducationCenterSystem.Api/"]
COPY ["src/EducationCenterSystem.Application/EducationCenterSystem.Application.csproj", "src/EducationCenterSystem.Application/"]
COPY ["src/EducationCenterSystem.Domain/EducationCenterSystem.Domain.csproj", "src/EducationCenterSystem.Domain/"]
COPY ["src/EducationCenterSystem.Infrastructure/EducationCenterSystem.Infrastructure.csproj", "src/EducationCenterSystem.Infrastructure/"]
RUN dotnet restore "./src/EducationCenterSystem.Api/EducationCenterSystem.Api.csproj"
COPY . .
WORKDIR "/src/src/EducationCenterSystem.Api"
RUN dotnet build "./EducationCenterSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EducationCenterSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EducationCenterSystem.Api.dll"]
