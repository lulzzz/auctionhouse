#!/bin/sh

echo "shard_1 init"
./main/shard_1/init.sh
echo "shard_2 init"
./main/shard_2/init.sh
echo "configdb init"
./config/init.sh
echo "mongos init"
./mongos/init.sh