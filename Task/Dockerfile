# Use the official .NET 9 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY Task/TaskManagement.sln .
COPY Task/TaskManagement/ ./TaskManagement/
RUN dotnet restore 

# Copy the rest of the application code
COPY Task/ .
WORKDIR /app/TaskManagement
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/TaskManagement/out .
ENTRYPOINT ["dotnet", "TaskManagement.dll"]