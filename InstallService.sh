sed "s+{path}+$(pwd)+;s+{user}+$USER+" CloudFileServer.service.template > CloudFileServer.service
mv -f CloudFileServer.service /etc/systemd/system || $(rm CloudFileServer.service && echo "error" && exit 125)

if [ -f "/etc/systemd/system/CloudFileServer.service" ]; then
    echo "Updating service"
    sudo systemctl daemon-reload
    sudo systemctl status CloudFileServer.service
else
    echo "Creating new service"
    sudo systemctl enable CloudFileServer.service
    sudo systemctl start CloudFileServer.service
    sudo systemctl status CloudFileServer.service
fi