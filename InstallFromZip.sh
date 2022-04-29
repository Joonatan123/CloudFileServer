#!/bin/bash
path=$1
zipFile=$2
if [ ! "${path: -1}" = "/" ]; then
    path="${path}/"
fi

if [ "${path}" = "/" ]; then
    echo "Invalid path"
    exit 1
fi

if [ ! -d "$path" ]; then
    echo "path doesn't exist"
    exit 125
fi

read -p "Install at $path y/n " -n 1 -r
echo    # (optional) move to a new line
if [[ ! $REPLY =~ ^[Yy]$ ]]
then
    exit 1
fi

echo "$path"

