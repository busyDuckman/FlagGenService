
FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app
RUN ls

RUN git clone https://github.com/busyDuckman/FlagGenService.git && \
    git clone https://github.com/busyDuckman/WDLibCore

WORKDIR /app/FlagGenService
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Copy csproj and restore as distinct layers
# COPY *.csproj ./


# Copy everything else and build
# COPY ../../ ./
# RUN dotnet publish -c Release -o out



# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/FlagGenService/FlagRestAPI/out .
ENTRYPOINT ["dotnet", "aspnetapp.dll"]