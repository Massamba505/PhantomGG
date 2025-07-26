FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app/server

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app/server

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PhantomGG.API.dll"]