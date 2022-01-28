FROM microsoft/aspnetcore-build AS builder

WORKDIR /code

COPY ./common/Shared ./common/Shared 
COPY ./common/Shared.Providers ./common/Shared.Providers 

COPY ./gameapps/Game.Minefield.Contracts ./gameapps/Game.Minefield.Contracts
COPY ./gameapps/Game.Minefield ./gameapps/Game.Minefield

COPY ./app/Chat/Chat.Contracts ./app/Chat/Chat.Contracts
COPY ./app/Payment.Contracts ./app/Payment.Contracts
COPY ./app/Persistance ./app/Persistance

COPY ./app/WebApi.Configuration ./app/WebApi.Configuration
COPY ./app/WebApi.Providers ./app/WebApi.Providers
COPY ./app/WebApi/ ./app/WebApi/
 
WORKDIR /code/app/WebApi/
RUN dotnet restore
RUN dotnet publish -c Release -o deploy

######
 
FROM microsoft/aspnetcore
WORKDIR /app

# copy from the build layer to the runtime layer
COPY --from=builder /code/app/WebApi/deploy /app
ENTRYPOINT [ "dotnet", "WebApi.dll" ]
