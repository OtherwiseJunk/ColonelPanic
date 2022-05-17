#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG TOKEN

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
ARG TOKEN
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG TOKEN
WORKDIR /src/
COPY ["ColonelPanic/ColonelPanic.csproj", "./"]
RUN dotnet restore "ColonelPanic.csproj"
COPY ["ColonelPanic.Database/ColonelPanic.Database.csproj", "./"]
RUN dotnet restore "ColonelPanic.Database.csproj"
COPY ["ColonelPanic.Modules/ColonelPanic.Modules.csproj", "./"]
RUN dotnet restore "ColonelPanic.Modules.csproj"
COPY ["ColonelPanic.Utilities/ColonelPanic.Utilities.csproj", "./"]
RUN dotnet restore "ColonelPanic.Utilities.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ColonelPanic.sln" -c Release -o /app/build

FROM build AS publish
ARG TOKEN
RUN dotnet publish "ColonelPanic.sln" -c Release -o /app/publish

FROM base AS final
ARG TOKEN
ENV COLONELPANIC=$TOKEN
WORKDIR /app
COPY --from=publish /app/publish .
RUN apt-get update
RUN apt-get install -y libfreetype6
RUN apt-get install -y libfontconfig1
RUN apt-get install -y libc6-dev 
RUN apt-get install -y libgdiplus
RUN rm /etc/localtime
RUN ln -s /usr/share/zoneinfo/America/New_York /etc/localtime
ENTRYPOINT ["dotnet", "ColonelPanic.dll"]
