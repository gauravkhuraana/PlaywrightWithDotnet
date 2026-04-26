# syntax=docker/dockerfile:1.7
ARG PLAYWRIGHT_VERSION=v1.49.0
FROM mcr.microsoft.com/playwright/dotnet:${PLAYWRIGHT_VERSION}-jammy AS build

WORKDIR /src
COPY . .

RUN dotnet restore PlaywrightWithDotnet.slnx
RUN dotnet build  PlaywrightWithDotnet.slnx -c Release --no-restore

FROM mcr.microsoft.com/playwright/dotnet:${PLAYWRIGHT_VERSION}-jammy AS runtime
ARG ALLURE_VERSION=2.30.0
ENV DOTNET_NOLOGO=1 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    TEST_ENV=qa \
    DEBIAN_FRONTEND=noninteractive

WORKDIR /work
COPY --from=build /src /work

# Optional: bundle Allure CLI for in-container report generation.
RUN apt-get update \
    && apt-get install -y --no-install-recommends openjdk-17-jre-headless curl ca-certificates unzip \
    && curl -sSLo /tmp/allure.zip https://github.com/allure-framework/allure2/releases/download/${ALLURE_VERSION}/allure-${ALLURE_VERSION}.zip \
    && unzip -q /tmp/allure.zip -d /opt \
    && ln -s /opt/allure-${ALLURE_VERSION}/bin/allure /usr/local/bin/allure \
    && rm -rf /tmp/allure.zip /var/lib/apt/lists/*

COPY docker/entrypoint.sh /usr/local/bin/entrypoint.sh
RUN chmod +x /usr/local/bin/entrypoint.sh

ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]
