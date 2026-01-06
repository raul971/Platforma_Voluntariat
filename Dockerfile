# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# Copy csproj and restore dependencies
COPY src/VolunteerFlow.Api/*.csproj ./src/VolunteerFlow.Api/
RUN dotnet restore ./src/VolunteerFlow.Api/VolunteerFlow.Api.csproj

# Copy everything else and build
COPY src/VolunteerFlow.Api/. ./src/VolunteerFlow.Api/
WORKDIR /source/src/VolunteerFlow.Api
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish .

# Create directory for SQLite database
RUN mkdir -p /app/data

# Expose port
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "VolunteerFlow.Api.dll"]
