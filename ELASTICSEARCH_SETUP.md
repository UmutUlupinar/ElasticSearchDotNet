# Elasticsearch Docker Kurulum Rehberi

## Sorun: Timeout Hatası

Eğer Elasticsearch'e bağlanırken timeout hatası alıyorsanız, aşağıdaki adımları izleyin:

## 1. Elasticsearch Container'ını Doğru Şekilde Başlatın

### Elasticsearch 8.x için (SSL olmadan):
```bash
docker run -d \
  --name elasticsearch \
  -p 9200:9200 \
  -p 9300:9300 \
  -e "discovery.type=single-node" \
  -e "xpack.security.enabled=false" \
  -e "xpack.security.enrollment.enabled=false" \
  -e "xpack.security.http.ssl.enabled=false" \
  -e "xpack.security.transport.ssl.enabled=false" \
  elasticsearch:8.11.0
```

### Elasticsearch 7.x için:
```bash
docker run -d \
  --name elasticsearch \
  -p 9200:9200 \
  -p 9300:9300 \
  -e "discovery.type=single-node" \
  elasticsearch:7.17.0
```

## 2. Container Durumunu Kontrol Edin

```bash
# Container'ın çalıştığını kontrol edin
docker ps

# Container loglarını kontrol edin
docker logs elasticsearch

# Container'ın hazır olduğunu kontrol edin (loglarda "started" mesajını arayın)
docker logs elasticsearch | grep started
```

## 3. Elasticsearch'e Erişimi Test Edin

### PowerShell ile:
```powershell
Invoke-WebRequest -Uri http://localhost:9200 -Method GET
```

### curl ile (Git Bash veya WSL):
```bash
curl http://localhost:9200
```

### Tarayıcı ile:
```
http://localhost:9200
```

Başarılı bir yanıt şöyle görünmelidir:
```json
{
  "name" : "...",
  "cluster_name" : "docker-cluster",
  "cluster_uuid" : "...",
  "version" : {
    "number" : "8.11.0",
    ...
  }
}
```

## 4. Eğer Hala Bağlanamıyorsanız

### Container'ı Yeniden Başlatın:
```bash
docker stop elasticsearch
docker rm elasticsearch
# Yukarıdaki docker run komutunu tekrar çalıştırın
```

### Port Çakışmasını Kontrol Edin:
```bash
# 9200 portunu kullanan başka bir process var mı?
netstat -ano | findstr :9200
```

### Docker Network Sorunu:
Eğer Docker Desktop kullanıyorsanız, WSL2 backend kullanıyorsanız `localhost` yerine container IP'sini kullanmanız gerekebilir:
```bash
# Container IP'sini bulun
docker inspect elasticsearch | grep IPAddress
```

Sonra `appsettings.json`'da:
```json
{
  "Elasticsearch": {
    "Uri": "http://172.17.0.2:9200"
  }
}
```

## 5. Windows Firewall Kontrolü

Windows Firewall'un 9200 portunu engellemediğinden emin olun.

## 6. Alternatif: Docker Compose Kullanımı

`docker-compose.yml` dosyası oluşturun:
```yaml
version: '3.8'
services:
  elasticsearch:
    image: elasticsearch:8.11.0
    container_name: elasticsearch
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
      - xpack.security.enrollment.enabled=false
      - xpack.security.http.ssl.enabled=false
      - xpack.security.transport.ssl.enabled=false
      - "ES_JAVA_OPTS=-Xms512m -Xmx512m"
    ports:
      - "9200:9200"
      - "9300:9300"
    volumes:
      - elasticsearch_data:/usr/share/elasticsearch/data

volumes:
  elasticsearch_data:
```

Çalıştırın:
```bash
docker-compose up -d
```

