#!/bin/bash
SERVICE_NAME="eventosvivos-api"
APP_DIR="/var/www/$SERVICE_NAME"
PORT=5035

echo "=========================================================="
echo "1. Configurando servicio systemd para la API pura ($SERVICE_NAME)..."
echo "=========================================================="

cat <<EOF > /etc/systemd/system/$SERVICE_NAME.service
[Unit]
Description=EventosVivos API .NET 9
After=network.target

[Service]
WorkingDirectory=$APP_DIR
ExecStart=/usr/bin/dotnet $APP_DIR/EventosVivos.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=$SERVICE_NAME
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://*:$PORT

[Install]
WantedBy=multi-user.target
EOF

echo "Recargando demonio de systemd y reiniciando API..."
systemctl daemon-reload
systemctl enable $SERVICE_NAME.service
systemctl restart $SERVICE_NAME.service


echo "=========================================================="
echo "2. Levantando el Proxy Puente para Traefik..."
echo "=========================================================="

# Nos aseguramos de estar en la carpeta donde subimos el docker-compose.proxy.yml
cd $APP_DIR

# Levantamos el contenedor proxy que Traefik va a detectar automáticamente
docker compose -f docker-compose.proxy.yml up -d

echo "=========================================================="
echo "¡Todo listo! Traefik ahora conoce el subdominio eventos-api."
echo "Prueba entrar a: http://eventos-api.cephasco.com/swagger"
echo "=========================================================="
