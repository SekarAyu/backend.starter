# backend.starter NHibernate Starter Project
This repository contain starter project based on NHibernate Framework, using **.net 6.0**.

## Features

- 🐋 **Docker** containerized (Micro services ready)
- 🤖 **Autofac** Dependency Injection
- 📨 **RabbitMQ** MassTransit pre-configured
- 🕒 **Quartz** Scheduler Job pre-configured
- 👣 **Log4Net** based logging
- 🪄 **Automapper** pre-configured
- 🕶️ **CORS** pre-configured
- 🚀 Using latest custom ORM NHibernate Framework (**QSI.ORM**)
- 🐙 Micro services framework ready
- 📖 Include Swagger UI documentation
- 🎨 Include Swagger API Versioning

## Internal Nexus Repository Authentication

Before running project, or running `dotnet restore` command you have to do several things:

- create `.env.secret` file or rename `.env.secret.example`
- add variable `QSI_REPO_USERNAME` and `QSI_REPO_PASSWORD` with appropriate value
- **OR** you can skip above steps, and create those variables on your machine env path

## CORS

By default CORS configuration will allow any origin, any header, and any method. To configure for specific domain, you can add or replace cors config on `configuration.yaml` file.

```yaml
# configuration.yaml
cors:
  credentials: true
  origins:
    - "http://mobile.dev.quadrant-si.id/cirrustavocadodev/"
    - "https://zurich-mzacms.dev.quadrant-si.id/"
  headers:
    - "*"
  methods:
    - "*"
```

## Date Time & Localization

- Please use `DateTime.UtcNow` instead `DateTime.Now` because by default DateTime object will be serialized to UTC ISO 8601 string format.
- Client / Consumer should parse the value from UTC Format to expected DateTime Time Zone offset

## Docker Container

### Docker Compose

- adjust config on .env file
- Please note that `host-gateway` is only available on Docker v20.10 and above, for Docker below that you need to specify docker host ip address (for more info please check [this SO Answer](https://stackoverflow.com/a/43541732))
- create file .env.secret, and add:

```env
QSI_REPO_USERNAME=<NEXUS REPO USERNAME>
QSI_REPO_PASSWORD=<NEXUS REPO PASSWORD>
```

- replace `<NEXUS REPO USERNAME>` and `<NEXUS REPO PASSWORD>` with appropriate values
- adjust some config on `.env` file (orm connection, publish runtime, etc)
- run:

  ```sh
  docker compose build # add --no-cache to rebuild without cache
  ```

- then run

  ```sh
  docker compose up
  ```

### Docker Build

- run command below:

```sh
docker build -t cirrust-avocado --build-arg PUBLISH_RUNTIME=<PUBLISH RUNTIME> --build-arg QSI_REPO_USERNAME=<NEXUS REPO USERNAME> --build-arg QSI_REPO_PASSWORD=<NEXUS REPO PASSWORD> --no-cache -f .\Cirrust.Avocado\Dockerfile .
```

- replace `<PUBLISH RUNTIME>`, `<NEXUS REPO USERNAME>`, `<NEXUS REPO PASSWORD>` with appropriate values

## Known Publish Runtime

You can check the list of known publish runtime id at [Microsoft Docs Page](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog#known-rids)

- Windows RIDs `win-x64`, `win-x86`, `win-arm64`
- Linux RIDs `linux-x64`, `linux-musl-x64`, `linux-musl-arm64`, `linux-arm`, `linux-arm64`, `linux-bionic-arm64`
- Mac OS RIDs `osx-x64`, `osx-arm64`
