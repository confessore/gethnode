#!/bin/sh

geth --ropsten --syncmode "light" --http --http.addr 0.0.0.0 --http.vhosts=* --ws --ws.addr="0.0.0.0" --ws.origins="*" --ws.api eth,net,web3,admin --maxpeers 30
