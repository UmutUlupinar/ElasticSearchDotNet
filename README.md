# ElasticSearchDotNet

Elasticsearch ile entegre edilmiş, Türkiye'nin şehir, ilçe ve mahalle verilerini yöneten ve arayan modern bir .NET uygulaması. Proje, ASP.NET Core Web API backend ve Blazor Server frontend'den oluşmaktadır.

## 📋 İçindekiler

- [Genel Bakış](#genel-bakış)
- [Özellikler](#özellikler)
- [Teknolojiler](#teknolojiler)
- [Kurulum](#kurulum)
- [Kullanım](#kullanım)
- [API Endpoint'leri](#api-endpointleri)
- [Elasticsearch Kurulumu](#elasticsearch-kurulumu)
- [Geliştirme](#geliştirme)

## 🎯 Genel Bakış

Bu proje, Elasticsearch kullanarak büyük ölçekli lokasyon verilerini (şehir, ilçe, mahalle) saklamak, yönetmek ve aramak için geliştirilmiştir. Uygulama, RESTful API ve modern bir web arayüzü sunar.

### Ana Bileşenler

- **ElasticSearchDotNet.Api**: RESTful API servisi
- **ElasticSearchDotNet.Web**: Blazor Server web uygulaması

## ✨ Özellikler

### API Özellikleri

- ✅ **Elasticsearch Entegrasyonu**: Tam Elasticsearch 8.x desteği
- ✅ **CRUD İşlemleri**: Şehir, ilçe ve mahalle verileri için tam CRUD desteği
- ✅ **Gelişmiş Arama**: Fuzzy search ve filtreleme ile güçlü arama özellikleri
- ✅ **Index Yönetimi**: Dinamik index oluşturma ve yönetimi
- ✅ **Toplu İşlemler**: Büyük veri setlerini verimli şekilde indexleme
- ✅ **Health Check**: Elasticsearch bağlantı ve cluster durumu kontrolü
- ✅ **Hata Yönetimi**: Merkezi exception handling middleware
- ✅ **API Dokümantasyonu**: Swagger/OpenAPI ile otomatik API dokümantasyonu
- ✅ **CORS Desteği**: Blazor frontend için yapılandırılmış CORS politikaları

### Web Özellikleri

- ✅ **Modern UI**: Bootstrap ile responsive tasarım
- ✅ **Blazor Server**: Gerçek zamanlı etkileşim
- ✅ **API Entegrasyonu**: HttpClient ile API servisleri

## 🛠 Teknolojiler

### Backend

- **.NET 10.0**: Modern .NET platformu
- **ASP.NET Core**: Web API framework
- **Elastic.Clients.Elasticsearch 8.11.0**: Resmi Elasticsearch .NET client
- **Swashbuckle.AspNetCore 10.0.1**: Swagger/OpenAPI dokümantasyonu
- **Microsoft.AspNetCore.OpenApi 10.0.0**: OpenAPI desteği

### Frontend

- **Blazor Server**: Interactive Server Components
- **Bootstrap**: CSS framework
- **ASP.NET Core Razor Components**: Component-based UI

### Altyapı

- **Elasticsearch 8.11.0**: Arama ve analitik motoru
- **Docker**: Containerization (opsiyonel)

## 🚀 Kurulum

### Gereksinimler

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/) (Elasticsearch için)
- [Git](https://git-scm.com/)

### Adımlar

1. **Projeyi klonlayın:**
   ```bash
   git clone <repository-url>
   cd ElasticSearchDotNet
   ```

2. **Bağımlılıkları yükleyin:**
   ```bash
   dotnet restore
   ```

3. **Elasticsearch'i başlatın:**
   
   Detaylı kurulum için `ELASTICSEARCH_SETUP.md` dosyasına bakın. Hızlı başlangıç:
   
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

4. **Elasticsearch bağlantısını test edin:**
   
   PowerShell ile:
   ```powershell
   .\test-elasticsearch.ps1
   ```
   
   Veya tarayıcıda: `http://localhost:9200`

5. **Yapılandırma:**
   
   `ElasticSearchDotNet.Api/appsettings.json` dosyasını kontrol edin:
   ```json
   {
     "Elasticsearch": {
       "Uri": "http://localhost:9200",
       "IndexPrefix": "location"
     }
   }
   ```

6. **Uygulamayı çalıştırın:**
   
   API'yi çalıştırın:
   ```bash
   cd ElasticSearchDotNet.Api
   dotnet run
   ```
   
   Web uygulamasını çalıştırın (yeni bir terminal):
   ```bash
   cd ElasticSearchDotNet.Web
   dotnet run
   ```

## 📖 Kullanım

### İlk Kurulum

1. **Index'leri oluşturun:**
   ```bash
   POST https://localhost:7267/api/index/create-indices
   ```

2. **Verileri indexleyin:**
   ```bash
   POST https://localhost:7267/api/index/index-all
   ```

   Bu komut JSON dosyalarından tüm verileri (şehirler, ilçeler, mahalleler) Elasticsearch'e yükler.

### API Kullanımı

API, Swagger UI üzerinden test edilebilir:
- **Swagger UI**: `https://localhost:5000/swagger/index.html`

### Örnek İstekler

#### Şehir Arama
```bash
curl -X GET "https://localhost:5000/api/location/cities/search?q=istanbul"
```

#### Şehir Koduna Göre İlçeler
```bash
curl -X GET "https://localhost:5000/api/location/districts?cityCode=34"
```

#### İlçe Arama
```bash
curl -X GET "https://localhost:5000/api/location/districts/search?q=kadıköy&cityCode=34"
```

### Örnek Yanıtlar

#### Başarılı Yanıt
```json
{
  "success": true,
  "data": [
    {
      "code": "34",
      "description": "İstanbul"
    }
  ],
  "message": null,
  "errors": null
}
```

#### Hata Yanıtı
```json
{
  "success": false,
  "data": null,
  "message": "Şehir bulunamadı",
  "errors": null
}
```

## 🔧 Elasticsearch Kurulumu

Detaylı kurulum ve sorun giderme için `ELASTICSEARCH_SETUP.md` dosyasına bakın.

### Hızlı Başlangıç

Elasticsearch 8.x için (SSL olmadan):
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

### Test

Elasticsearch bağlantısını test etmek için:
```powershell
.\test-elasticsearch.ps1
```

## 💻 Geliştirme

### Geliştirme Ortamı

- **IDE**: JetBrains Rider veya Visual Studio 2022
- **.NET SDK**: 10.0 veya üzeri
- **Docker**: Elasticsearch için

### Proje Yapısı

- **Services**: İş mantığı katmanı
- **Controllers**: API endpoint'leri
- **Models**: Veri modelleri
- **Middleware**: HTTP pipeline middleware'leri

### Loglama

Uygulama, Microsoft.Extensions.Logging kullanarak loglama yapar. Log seviyeleri `appsettings.json` dosyasında yapılandırılabilir.

### Hata Yönetimi

Tüm hatalar `ExceptionHandlingMiddleware` tarafından yakalanır ve standart `ApiResponse` formatında döner.

### CORS Yapılandırması

Blazor frontend için CORS yapılandırması `Program.cs` dosyasında tanımlanmıştır:
- Development: `https://localhost:7247`, `http://localhost:5029`


## 📝 Lisans

Bu proje açık kaynaklıdır ve MIT lisansı altında lisanslanmıştır.

## 🤖 Kullanılan Yapay Zeka Araçları

Bu proje geliştirilirken aşağıdaki yapay zeka destekli araçlar kullanılmıştır:

- **Cursor IDE Chat 5.1**: Kod yazımı, refactoring, hata ayıklama ve proje yapısı geliştirme süreçlerinde kullanılmıştır.
- **GitHub Copilot (Rider IDE)**: Kod tamamlama, örnek kod üretimi ve geliştirme hızlandırma için JetBrains Rider IDE üzerinden entegre edilmiştir.

Bu araçlar, projenin geliştirilmesi sırasında kod kalitesini artırmak, geliştirme süresini kısaltmak ve en iyi pratikleri uygulamak için yardımcı olmuştur.
