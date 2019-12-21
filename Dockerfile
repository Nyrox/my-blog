FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet publish -c Release -o out backend

FROM mcr.microsoft.com/dotnet/core/runtime:3.0
EXPOSE 8080
WORKDIR /app
COPY --from=build-env /app/ .
ENTRYPOINT ["/app/out/backend"]
