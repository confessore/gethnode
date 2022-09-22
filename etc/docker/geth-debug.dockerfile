FROM ubuntu:20.04
RUN apt-get update
RUN apt-get install -y software-properties-common
RUN add-apt-repository -y ppa:ethereum/ethereum
RUN apt-get update
RUN apt-get install -y ethereum
COPY scripts/geth-entrypoint-debug.sh .
RUN chmod +x ./geth-entrypoint-debug.sh
ENTRYPOINT ["./geth-entrypoint-debug.sh"]