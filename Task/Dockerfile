# Use the official .NET 9 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory
WORKDIR /src

# Copy the solution file
COPY Task/TaskManagement.sln .

# Copy the project directory
COPY Task/TaskManagementApp/ ./TaskManagementApp/

# Restore dependencies
RUN dotnet restore "TaskManagement.sln"

# Copy the rest of the source code
COPY Task/ .

# Publish the application
RUN dotnet publish "TaskManagementApp/TaskManagement.csproj" -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskManagement.dll"]