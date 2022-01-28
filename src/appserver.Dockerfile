# layer for building the image
FROM microsoft/dotnet:sdk AS builder
 
WORKDIR /code
COPY ./app/AppServer ./app/AppServer

COPY ./common/Akka.Quartz.Actor ./common/Akka.Quartz.Actor
COPY ./common/Shared ./common/Shared
COPY ./common/Shared.Providers ./common/Shared.Providers

COPY ./gameapps/Game.Minefield.Contracts ./gameapps/Game.Minefield.Contracts
COPY ./gameapps/Game.Minefield ./gameapps/Game.Minefield

COPY ./app/Chat/Chat.Contracts ./app/Chat/Chat.Contracts
COPY ./app/Chat/Chat ./app/Chat/Chat

COPY ./app/Payment.Contracts ./app/Payment.Contracts
COPY ./app/Payment ./app/Payment

COPY ./app/Persistance ./app/Persistance
 
WORKDIR /code/app/AppServer
RUN dotnet restore
RUN dotnet publish -c Release -o deploy
 
##################

FROM microsoft/dotnet:runtime

### ENABLE CONTAINER DEBUGGING # Installing vsdbg debbuger into our container 
#WORKDIR /vsdbg
#RUN apt-get update \
    #&& apt-get install -y --no-install-recommends \
       #unzip \
    #&& rm -rf /var/lib/apt/lists/* \
    #&& curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

#### copy from the build layer to the runtime layer
WORKDIR /code
COPY --from=builder /code/app/AppServer/deploy /app
 
WORKDIR /app
EXPOSE 8085/tcp
ENTRYPOINT [ "dotnet", "AppServer.dll"]
