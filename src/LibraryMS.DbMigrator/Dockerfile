FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["common.props", "./"]
COPY ["src/LibraryMS.Domain.Shared/LibraryMS.Domain.Shared.csproj", "src/LibraryMS.Domain.Shared/"]
COPY ["src/LibraryMS.Domain/LibraryMS.Domain.csproj", "src/LibraryMS.Domain/"]
COPY ["src/LibraryMS.Application.Contracts/LibraryMS.Application.Contracts.csproj", "src/LibraryMS.Application.Contracts/"]
COPY ["src/LibraryMS.Application/LibraryMS.Application.csproj", "src/LibraryMS.Application/"]
COPY ["src/LibraryMS.EntityFrameworkCore/LibraryMS.EntityFrameworkCore.csproj", "src/LibraryMS.EntityFrameworkCore/"]
COPY ["src/LibraryMS.Infrastructure/LibraryMS.Infrastructure.csproj", "src/LibraryMS.Infrastructure/"]
COPY ["src/LibraryMS.DbMigrator/LibraryMS.DbMigrator.csproj", "src/LibraryMS.DbMigrator/"]

RUN dotnet restore "src/LibraryMS.DbMigrator/LibraryMS.DbMigrator.csproj"

COPY . .
WORKDIR "/src/src/LibraryMS.DbMigrator"
RUN dotnet build "LibraryMS.DbMigrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LibraryMS.DbMigrator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibraryMS.DbMigrator.dll"]
