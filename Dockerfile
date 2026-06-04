FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY proto/contracts/ proto/contracts/
COPY "src/Training.AI/Training.AI.csproj" "src/Training.AI/"
RUN dotnet restore "src/Training.AI/Training.AI.csproj"
COPY . .
RUN dotnet publish "src/Training.AI/Training.AI.csproj" -c Release -o /out

FROM alpine:3.21 AS goose
RUN apk add --no-cache curl && \
    curl -sL https://github.com/pressly/goose/releases/download/v3.24.1/goose-linux-amd64 -o /goose && \
    chmod +x /goose

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
RUN apt-get update && apt-get install -y --no-install-recommends postgresql-client && rm -rf /var/lib/apt/lists/*
COPY --from=goose /goose /usr/local/bin/goose
COPY --from=build /out /app
COPY --from=build /src/src/Training.AI.Domain/Migrations /app/Migrations
WORKDIR /app
EXPOSE 5004
ENV ASPNETCORE_URLS=http://+:5004
ENV ASPNETCORE_ENVIRONMENT=Production
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh
ENTRYPOINT ["/docker-entrypoint.sh"]
