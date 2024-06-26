FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy files
COPY . .
RUN dotnet restore
RUN dotnet build -c Release

# Copy and publish app and libraries
WORKDIR /source/main/GarnetServer
RUN dotnet publish -c Release -o /app --self-contained false -f net8.0

# Final stage/image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .

# Run GarnetServer with an index size of 128MB
ENTRYPOINT ["/app/GarnetServer", "-i", "128m"]