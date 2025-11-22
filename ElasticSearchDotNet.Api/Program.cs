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
