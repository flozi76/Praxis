FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
#COPY ["Duftfinder.Web/Duftfinder.Web.csproj", "Duftfinder.Web/"]
#RUN dotnet restore "Duftfinder.Web/Duftfinder.Web.csproj"
COPY . /.

RUN dotnet restore "Duftfinder.Web/*.csproj"
WORKDIR "/src/Duftfinder.Web"
RUN dotnet build "Duftfinder.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Duftfinder.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "duftfinder.web.dll"]