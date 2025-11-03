FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY PhantomGG.sln ./
COPY PhantomGG.API/PhantomGG.API.csproj ./PhantomGG.API/
COPY PhantomGG.Common/PhantomGG.Common.csproj ./PhantomGG.Common/
COPY PhantomGG.Models/PhantomGG.Models.csproj ./PhantomGG.Models/
COPY PhantomGG.Repository/PhantomGG.Repository.csproj ./PhantomGG.Repository/
COPY PhantomGG.Service/PhantomGG.Service.csproj ./PhantomGG.Service/
COPY PhantomGG.Validation/PhantomGG.Validation.csproj ./PhantomGG.Validation/
COPY PhantomGG.UnitTests/PhantomGG.UnitTests.csproj ./PhantomGG.UnitTests/

RUN dotnet restore PhantomGG.sln

COPY . . 

RUN dotnet publish PhantomGG.sln -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /out ./

EXPOSE 5000

ENTRYPOINT ["dotnet", "PhantomGG.API.dll"]
