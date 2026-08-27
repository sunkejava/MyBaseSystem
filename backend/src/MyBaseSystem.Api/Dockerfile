FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish backend/src/MyBaseSystem.Api/MyBaseSystem.Api.csproj -c Release -o /out --no-self-contained
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MyBaseSystem.Api.dll"]
