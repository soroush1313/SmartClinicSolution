FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AppointmentService.API/AppointmentService.API.csproj", "AppointmentService.API/"]
COPY ["AppointmentService.Application/AppointmentService.Application.csproj", "AppointmentService.Application/"]
COPY ["AppointmentService.Infrastructure/AppointmentService.Infrastructure.csproj", "AppointmentService.Infrastructure/"]
RUN dotnet restore "AppointmentService.API/AppointmentService.API.csproj"

COPY . .
WORKDIR "/src/AppointmentService.API"
RUN dotnet build "AppointmentService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppointmentService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentService.API.dll"]
