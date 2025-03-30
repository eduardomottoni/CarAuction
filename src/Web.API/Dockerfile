FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Web.API.csproj ./
RUN dotnet restore "Web.API.csproj"

COPY . .
WORKDIR "/src"

RUN dotnet build "Web.API.csproj" -c Release -o /app/build
RUN dotnet test "Web.API.csproj" -c Release
RUN dotnet publish "Web.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Web.API.dll"]
