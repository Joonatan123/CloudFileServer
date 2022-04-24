#!/bin/bash
path=$1
if [ ! "${path: -1}" = "/" ]; then
    path="${path}/"
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
cp -r * $path
echo "${path}appsettings.json"
if [ ! -f "${path}appsettings.json" ]; then
    echo "No config found"
    cp "${path}appsettings.json.template" "${path}appsettings.json"
else
    echo "Old config found"
fi