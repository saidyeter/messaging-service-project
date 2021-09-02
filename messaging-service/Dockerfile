FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
ENV TZ=Europe/Istanbul
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
   
COPY ./MessagingService.Api/MessagingService.Api.csproj ./MessagingService.Api/
COPY ./MessagingService.MongoDB/MessagingService.MongoDB.csproj ./MessagingService.MongoDB/
RUN dotnet restore ./MessagingService.Api/MessagingService.Api.csproj

COPY . .
RUN dotnet build ./MessagingService.Api/MessagingService.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ./MessagingService.Api/MessagingService.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessagingService.Api.dll"]