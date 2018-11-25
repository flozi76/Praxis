FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Duftfinder/Duftfinder.csproj", "Duftfinder/"]
RUN dotnet restore "Duftfinder/Duftfinder.csproj"
COPY . .
WORKDIR "/src/Duftfinder"
RUN dotnet build "Duftfinder.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Duftfinder.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Duftfinder.dll"]