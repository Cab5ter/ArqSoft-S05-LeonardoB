using CitasApp.Application.Services;
using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "CitasApp API",
        Version = "v1",
        Description = "API para gestión de citas médicas y calculadora"
    });
});

// Repositorios
builder.Services.AddScoped<IPacienteRepository, JsonPacienteRepository>();
builder.Services.AddScoped<IMedicoRepository,   JsonMedicoRepository>();
builder.Services.AddScoped<ICitaRepository,     JsonCitaRepository>();

// Servicios de aplicación
builder.Services.AddScoped<PacienteService>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<CitaService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CitasApp API V1");
    c.RoutePrefix = "docs";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
