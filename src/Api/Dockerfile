# sudo docker build -t core-divdados -f src/Api/Dockerfile .;
# sudo docker tag core-divdados:latest core-divdados:1.0.0;
# aws ecr get-login-password --region sa-east-1 | sudo docker login --username AWS --password-stdin 463346631047.dkr.ecr.sa-east-1.amazonaws.com/core-divdados;
# sudo docker tag core-divdados:1.0.0 463346631047.dkr.ecr.sa-east-1.amazonaws.com/core-divdados:1.0.0;
# sudo docker push 463346631047.dkr.ecr.sa-east-1.amazonaws.com/core-divdados:1.0.0;
# sudo docker run -p 5000:5000 core-divdados;

FROM mcr.microsoft.com/dotnet/aspnet:7.0.0-alpine3.16 AS base
WORKDIR "/app"
EXPOSE 80

RUN apk add --no-cache icu-libs
RUN apk add --no-cache icu-data-full
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:7.0.100-alpine3.16 AS build
COPY . .
RUN dotnet restore "src/Api/Core.Divdados.Api.csproj"
RUN dotnet build "src/Api/Core.Divdados.Api.csproj" -c Release -o /build

FROM build AS publish
RUN dotnet publish "src/Api/Core.Divdados.Api.csproj" -c Release -o /publish

FROM base AS final
COPY --from=publish /publish .

ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Local
EXPOSE 5000
ENTRYPOINT ["dotnet", "Core.Divdados.Api.dll", "--server.urls", "http://*:5000"]