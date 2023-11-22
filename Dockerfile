#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SpellingTest.Core/SpellingTest.Core.csproj", "./SpellingTest.Core/"]
COPY ["SpellingTest.Blazor/SpellingTest.Web.csproj", "./SpellingTest.Blazor/"]
COPY ["NuGet.Config", "."]

ARG PAT=nokeypassed
RUN sed -i "s|</configuration>|<packageSourceCredentials><Polyhydra><add key=\"Username\" value=\"PAT\" /><add key=\"ClearTextPassword\" value=\"${PAT}\" /></Polyhydra><Base><add key=\"Username\" value=\"PAT\" /><add key=\"ClearTextPassword\" value=\"${PAT}\" /></Base></packageSourceCredentials></configuration>|" NuGet.Config
RUN cat NuGet.Config

RUN dotnet restore "./SpellingTest.Core/SpellingTest.Core.csproj" --configfile NuGet.Config
COPY ./SpellingTest.Core ./SpellingTest.Core
WORKDIR "/src/SpellingTest.Core/."
RUN dotnet build "SpellingTest.Core/SpellingTest.Core.csproj" -c Release -o /app/build


RUN dotnet restore "./SpellingTest.Blazor/SpellingTest.Web.csproj" --configfile NuGet.Config
COPY SpellingTest.Blazor/. SpellingTest.Blazor/.
WORKDIR "/src/SpellingTest.Blazor/."
RUN dotnet build "SpellingTest.Blazor/SpellingTest.Web.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "SpellingTest.Blazor/SpellingTest.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpellingTest.Blazor.dll"]