using MotoNav.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. Controller ve Swagger Servisleri
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Katman Servis Kayıtları (Persistence)
builder.Services.AddPersistenceServices(builder.Configuration);

// 3. SignalR Servisi
builder.Services.AddSignalR();

// 4. CORS Politikası (SignalR WebSocket bağlantıları için)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
});

var app = builder.Build();

// 5. Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

// 6. Endpoint ve Hub Yönlendirmeleri
app.MapControllers();
app.MapHub<moto_nav_back.Hubs.RideHub>("/hubs/ride");

app.Run(); 