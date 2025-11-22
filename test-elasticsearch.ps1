# Elasticsearch Bağlantı Test Scripti

Write-Host "Elasticsearch Bağlantı Testi" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# 1. Container durumunu kontrol et
Write-Host "1. Docker Container Durumu:" -ForegroundColor Yellow
$container = docker ps -a --filter "name=elasticsearch" --format "{{.Names}}"
if ($container) {
    Write-Host "   Container bulundu: $container" -ForegroundColor Green
    $status = docker ps --filter "name=elasticsearch" --format "{{.Status}}"
    if ($status) {
        Write-Host "   Durum: $status" -ForegroundColor Green
    } else {
        Write-Host "   Container durdurulmuş!" -ForegroundColor Red
        Write-Host "   Container'ı başlatmak için: docker start elasticsearch" -ForegroundColor Yellow
    }
} else {
    Write-Host "   Container bulunamadı!" -ForegroundColor Red
    Write-Host "   Container oluşturmak için ELASTICSEARCH_SETUP.md dosyasına bakın" -ForegroundColor Yellow
}

Write-Host ""

# 2. Port kontrolü
Write-Host "2. Port 9200 Kontrolü:" -ForegroundColor Yellow
$port = netstat -ano | findstr :9200
if ($port) {
    Write-Host "   Port 9200 kullanımda:" -ForegroundColor Green
    Write-Host "   $port" -ForegroundColor Gray
} else {
    Write-Host "   Port 9200 boş" -ForegroundColor Red
}

Write-Host ""

# 3. HTTP bağlantı testi
Write-Host "3. HTTP Bağlantı Testi:" -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:9200" -Method GET -TimeoutSec 5 -ErrorAction Stop
    Write-Host "   Bağlantı başarılı!" -ForegroundColor Green
    Write-Host "   Status Code: $($response.StatusCode)" -ForegroundColor Green
    $json = $response.Content | ConvertFrom-Json
    Write-Host "   Cluster Name: $($json.cluster_name)" -ForegroundColor Green
    Write-Host "   Version: $($json.version.number)" -ForegroundColor Green
} catch {
    Write-Host "   Bağlantı başarısız!" -ForegroundColor Red
    Write-Host "   Hata: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "   Çözüm önerileri:" -ForegroundColor Yellow
    Write-Host "   1. Container'ı başlatın: docker start elasticsearch" -ForegroundColor Yellow
    Write-Host "   2. Container loglarını kontrol edin: docker logs elasticsearch" -ForegroundColor Yellow
    Write-Host "   3. ELASTICSEARCH_SETUP.md dosyasına bakın" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Test tamamlandı!" -ForegroundColor Cyan

