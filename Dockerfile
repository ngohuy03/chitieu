# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy solution and project files
COPY GroupExpenseManager.sln ./
COPY GroupExpenseManager.Api/GroupExpenseManager.Api.csproj GroupExpenseManager.Api/
COPY GroupExpenseManager.Application/GroupExpenseManager.Application.csproj GroupExpenseManager.Application/
COPY GroupExpenseManager.Domain/GroupExpenseManager.Domain.csproj GroupExpenseManager.Domain/
COPY GroupExpenseManager.Infrastructure/GroupExpenseManager.Infrastructure.csproj GroupExpenseManager.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else and build release
COPY . ./
RUN dotnet publish GroupExpenseManager.Api/GroupExpenseManager.Api.csproj -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose port 8080 (Render default or configurable)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GroupExpenseManager.Api.dll"]
