using CitasApp.Application.Services;
using CitasApp.Data;
using CitasApp.Interfaces;
using CitasApp.Infrastructure.Repositories;
using CitasApp.Infrastructure.Observers;
using CitasApp.Infrastructure.Security;
using CitasApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Autenticación por cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Denegado";
    });

// Puertos y adaptadores de autenticación (arquitectura hexagonal + DIP)
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IUsuarioRepository>(sp =>
{
    var env      = sp.GetRequiredService<IWebHostEnvironment>();
    var dataPath = Path.Combine(env.ContentRootPath, "Data", "json");
    return new JsonUsuarioRepository(dataPath);
});
builder.Services.AddScoped<AuthService>();

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
builder.Services.AddSingleton<ICitaConsulta, CitaConsulta>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

DatosApp.Inicializar(app.Environment.ContentRootPath);

// Seed de usuario demo (idempotente): admin@citasapp.com / Admin123
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
    var auth = scope.ServiceProvider.GetRequiredService<AuthService>();
    if (repo.ObtenerPorEmail("admin@citasapp.com") is null)
        auth.Registrar("Administrador", "admin@citasapp.com", "Admin123", "Admin");
}

app.Run();
