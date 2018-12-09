FROM microsoft/dotnet:2.1.0-runtime-stretch-slim-arm32v7

WORKDIR /application
COPY build/linux-arm/ .

# NOTE Currently we do not have any static files that should be included in the docker image. Keep this in case we need it later.
# COPY deploymentfiles/ ./data/

EXPOSE 9090

ENTRYPOINT ["./FoodDatabase"]