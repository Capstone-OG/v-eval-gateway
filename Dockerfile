# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy gRPC definitions and Gateway project file
COPY ["grpc/", "grpc/"]
COPY ["All Services/V-Eval-Gateway/V-Eval-Gateway.API/V-Eval-Gateway.API.csproj", "All Services/V-Eval-Gateway/V-Eval-Gateway.API/"]
RUN dotnet restore "All Services/V-Eval-Gateway/V-Eval-Gateway.API/V-Eval-Gateway.API.csproj"

# Copy full source and publish Release artifact
COPY ["All Services/V-Eval-Gateway/", "All Services/V-Eval-Gateway/"]
WORKDIR "/src/All Services/V-Eval-Gateway/V-Eval-Gateway.API"
RUN dotnet publish "V-Eval-Gateway.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: ASP.NET Core Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 5212
ENV ASPNETCORE_URLS=http://+:5212
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "V-Eval-Gateway.API.dll"]
