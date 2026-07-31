using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Data;
using RegistrosInstitucionales.Api.Repositories;
using RegistrosInstitucionales.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("SviDb")
        ?? throw new InvalidOperationException(
            "No se encontró la cadena de conexión 'SviDb'.");

    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IReporteRepository, ReporteRepository>();
builder.Services.AddScoped<IEntidadRepository, EntidadRepository>();
builder.Services.AddScoped<IRegistroRepository, RegistroRepository>();
builder.Services.AddScoped<ILogAccesoRepository, LogAccesoRepository>();
builder.Services.AddScoped<IRegistroConsultaService, RegistroConsultaService>();
builder.Services.AddScoped<IEntidadRegistroService, EntidadRegistroService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendDev");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(context);
}

app.Run();

public partial class Program;
