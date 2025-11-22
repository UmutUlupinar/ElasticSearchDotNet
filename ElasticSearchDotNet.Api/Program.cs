var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Swagger/OpenAPI configuration
builder.Services.AddSwaggerGen();

// CORS configuration for Blazor frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7247", "http://localhost:5029")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Elasticsearch Client
var elasticsearchUri = builder.Configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
builder.Services.AddSingleton<Elastic.Clients.Elasticsearch.ElasticsearchClient>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Elasticsearch URI: {Uri}", elasticsearchUri);
    
    try
    {
        var uri = new Uri(elasticsearchUri);
        var settings = new Elastic.Clients.Elasticsearch.ElasticsearchClientSettings(uri)
            .DisableDirectStreaming()
            .RequestTimeout(TimeSpan.FromSeconds(10)); // Response stream'i yakalamak için
        
        // SSL sertifika doğrulamasını devre dışı bırak (development için)
        // Production'da bu ayarı kaldırın ve doğru sertifikaları kullanın
        if (builder.Environment.IsDevelopment())
        {
            settings.ServerCertificateValidationCallback((o, certificate, chain, errors) => true);
        }
        
        var client = new Elastic.Clients.Elasticsearch.ElasticsearchClient(settings);
        
        // Başlangıçta bağlantıyı test et (asenkron, uygulama başlamasını engellemez)
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(2000); // 2 saniye bekle, container hazır olsun
                var pingResponse = await client.PingAsync();
                if (pingResponse.IsValidResponse)
                {
                    logger.LogInformation("Elasticsearch bağlantısı başarılı: {Uri}", elasticsearchUri);
                }
                else
                {
                    logger.LogWarning("Elasticsearch bağlantısı başarısız: {Error}", pingResponse.DebugInformation);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Elasticsearch bağlantı testi başarısız: {Uri}. Hata: {Message}", elasticsearchUri, ex.Message);
            }
        });
        
        return client;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Elasticsearch client oluşturulurken hata: {Message}", ex.Message);
        throw;
    }
});

// Elasticsearch Services
builder.Services.AddScoped<ElasticSearchDotNet.Api.Services.IElasticsearchService, ElasticSearchDotNet.Api.Services.ElasticsearchService>();

// Location Data Service - Elasticsearch kullanacak şekilde yapılandırıldı
// JSON dosyalarından okumak için: LocationDataService
// Elasticsearch'ten okumak için: ElasticsearchLocationDataService
builder.Services.AddSingleton<ElasticSearchDotNet.Api.Services.ILocationDataService, ElasticSearchDotNet.Api.Services.ElasticsearchLocationDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElasticSearch DotNet API v1");
        c.RoutePrefix = "swagger";
    });
    app.MapOpenApi();
}

// Exception handling middleware (should be early in pipeline)
app.UseMiddleware<ElasticSearchDotNet.Api.Middleware.ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthorization();
app.MapControllers();

app.Run();
