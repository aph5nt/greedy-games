#!/bin/sh
if docker network ls | grep "greedygames-network" ; then echo "found network"; else docker network create --driver bridge --subnet 10.11.140.0/24 --gateway 10.11.140.1 --opt com.docker.network.bridge.name:greedygames-network greedygames-network; fi
