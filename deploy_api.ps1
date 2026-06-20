# Script de despliegue automático para API EventosVivos
$VPS_IP = "72.61.70.56"
$LOCAL_ROOT_PATH = "c:\Users\ingen\OneDrive\Escritorio\Angular\CEIBA\EV_BACK\EventosVivos"
$LOCAL_PUBLISH_PATH = "$LOCAL_ROOT_PATH\PUBLISH_API"
$SETUP_SCRIPT_PATH = "$LOCAL_ROOT_PATH\setup-vps.sh"
$REMOTE_DIR = "/var/www/eventosvivos-api"

Write-Host "==========================================================" -ForegroundColor Green
Write-Host "Paso Previo: Compilando aplicación..." -ForegroundColor Cyan
dotnet publish "$LOCAL_ROOT_PATH\EventosVivos.Api\EventosVivos.Api.csproj" -c Release -o $LOCAL_PUBLISH_PATH

Write-Host "==========================================================" -ForegroundColor Green
Write-Host "Iniciando despliegue de API EventosVivos a VPS ($VPS_IP)" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green

# 1. Crear carpeta remota
Write-Host "[1/4] Creando directorio remoto $REMOTE_DIR..." -ForegroundColor Cyan
ssh root@$VPS_IP "mkdir -p $REMOTE_DIR"

# 2. Subir los archivos compilados
Write-Host "[2/4] Subiendo archivos compilados de la API..." -ForegroundColor Cyan
# Limpiar archivos remotos existentes para asegurar despliegue limpio
ssh root@$VPS_IP "rm -rf $REMOTE_DIR/*"
# Subir archivos
scp -r "$LOCAL_PUBLISH_PATH\*" "root@${VPS_IP}:$REMOTE_DIR/"

# 3. Subir script y configuraciones de proxy
Write-Host "[3/4] Subiendo script de configuración del servidor..." -ForegroundColor Cyan
scp "$LOCAL_ROOT_PATH\setup-vps.sh" "root@${VPS_IP}:$REMOTE_DIR/"
scp "$LOCAL_ROOT_PATH\nginx-proxy.conf" "root@${VPS_IP}:$REMOTE_DIR/"
scp "$LOCAL_ROOT_PATH\docker-compose.proxy.yml" "root@${VPS_IP}:$REMOTE_DIR/"

# 4. Ejecutar la configuración y arranque en el servidor VPS
Write-Host "[4/4] Ejecutando configuración en el servidor..." -ForegroundColor Cyan
ssh root@$VPS_IP "chmod +x $REMOTE_DIR/setup-vps.sh && sudo bash $REMOTE_DIR/setup-vps.sh"

Write-Host "==========================================================" -ForegroundColor Green
Write-Host "¡Despliegue finalizado exitosamente!" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
