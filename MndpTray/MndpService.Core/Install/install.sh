#!/usr/bin/env bash
#set -v

#-----------------------------------------------------------------------------
# * Project:    MndpTray
# * Repository: https://github.com/xmegz/MndpTray
# * Author:     Pádár Tamás
# ----------------------------------------------------------------------------

PROGRAM_PATH="/root/MndpService.Core"
PROGRAM_NAME="MndpService.Core.Full"
SERVICE_NAME="mndp"
SERVICE_VERSION="2.3.0"

IS_ACTIVE=$(sudo systemctl is-active $SERVICE_NAME)
if [ "$IS_ACTIVE" == "active" ]; then

echo "Service is running, stopping it..."
echo

systemctl disable $SERVICE_NAME
systemctl stop $SERVICE_NAME
systemctl status $SERVICE_NAME

fi

echo
echo "Downloading service..."
echo

mkdir -p $PROGRAM_PATH
wget -O $PROGRAM_PATH/$PROGRAM_NAME https://github.com/xmegz/MndpTray/releases/download/v$SERVICE_VERSION/MndpService.Core.Full
chmod +x $PROGRAM_PATH/$PROGRAM_NAME


echo
echo "Creating systemd service..."
echo

cat > /etc/systemd/system/mndp.service << EOF
[Unit]
Description=MikroTik Neighbor Discovery Protocol Service
After=network.target

[Service]
WorkingDirectory=/root/MndpService.Core
ExecStart=/root/MndpService.Core/MndpService.Core.Full
Restart=always
RestartSec=15
KillSignal=SIGINT
SyslogIdentifier=mndp
User=root
Group=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOF


echo
echo "Starting systemd service..."
echo

systemctl daemon-reload
systemctl enable $SERVICE_NAME
systemctl start $SERVICE_NAME
systemctl status $SERVICE_NAME
