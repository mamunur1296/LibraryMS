FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["common.props", "./"]
COPY ["src/LibraryMS.Domain.Shared/LibraryMS.Domain.Shared.csproj", "src/LibraryMS.Domain.Shared/"]
COPY ["src/LibraryMS.Domain/LibraryMS.Domain.csproj", "src/LibraryMS.Domain/"]
COPY ["src/LibraryMS.Application.Contracts/LibraryMS.Application.Contracts.csproj", "src/LibraryMS.Application.Contracts/"]
COPY ["src/LibraryMS.Application/LibraryMS.Application.csproj", "src/LibraryMS.Application/"]
COPY ["src/LibraryMS.EntityFrameworkCore/LibraryMS.EntityFrameworkCore.csproj", "src/LibraryMS.EntityFrameworkCore/"]
COPY ["src/LibraryMS.Infrastructure/LibraryMS.Infrastructure.csproj", "src/LibraryMS.Infrastructure/"]
COPY ["src/LibraryMS.HttpApi/LibraryMS.HttpApi.csproj", "src/LibraryMS.HttpApi/"]
COPY ["src/LibraryMS.HttpApi.Host/LibraryMS.HttpApi.Host.csproj", "src/LibraryMS.HttpApi.Host/"]

RUN dotnet restore "src/LibraryMS.HttpApi.Host/LibraryMS.HttpApi.Host.csproj"

COPY . .
WORKDIR "/src/src/LibraryMS.HttpApi.Host"
RUN dotnet build "LibraryMS.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LibraryMS.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibraryMS.HttpApi.Host.dll"]
