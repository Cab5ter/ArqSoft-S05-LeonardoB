using CitasApp.Application.Services;
using CitasApp.Data;
using CitasApp.Interfaces;
using CitasApp.Infrastructure.Repositories;
using CitasApp.Infrastructure.Observers;
using CitasApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Factory + Decorator (Paciente)
builder.Services.AddScoped<IPacienteRepository>(sp =>
{
    var env      = sp.GetRequiredService<IWebHostEnvironment>();
    var dataPath = Path.Combine(env.ContentRootPath, "Data", "json");
    var repo     = RepositoryFactory.CrearPacienteRepository(
                       builder.Environment.EnvironmentName, dataPath);
    return new LoggingPacienteRepository(repo);
});

// Observer (Cita) — CitaService no conoce a SmsObserver ni EmailObserver
builder.Services.AddSingleton<CitaService>(sp =>
{
    var service = new CitaService();
    service.Suscribir(new SmsObserver());
    service.Suscribir(new EmailObserver());
    return service;
});

// CitaServicio para CRUD de citas
builder.Services.AddSingleton<CitaServicio>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

DatosApp.Inicializar(app.Environment.ContentRootPath);

app.Run();
