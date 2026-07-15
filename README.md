# CitasApp — refactorización con Extract Class, DI y SOLID

## Descripción

CitasApp es una aplicación ASP.NET Core para administrar pacientes, médicos y citas médicas. En este proyecto implementé operaciones de consulta y mantenimiento, persistencia en archivos JSON, autenticación mediante cookies y notificaciones simuladas cuando una cita es confirmada.

El objetivo principal de este trabajo fue refactorizar el módulo de citas mediante **Extract Class** e **inyección de dependencias (DI)**, manteniendo el comportamiento de la aplicación y mejorando el cumplimiento de los principios **SOLID**.

## Funcionalidades

- Registro, consulta y edición de pacientes.
- Registro, consulta y edición de médicos.
- Creación, edición, eliminación y filtrado de citas por paciente.
- Estados de cita: pendiente, confirmada y cancelada.
- Persistencia local mediante archivos JSON.
- Inicio y cierre de sesión con autenticación por cookies.
- Notificaciones simuladas por SMS y correo al confirmar una cita.
- API REST para consultar pacientes, médicos y citas.

## Problema antes de la refactorización

`CitaController` tenía más responsabilidades de las que corresponden a un controlador MVC. Además de recibir peticiones HTTP y seleccionar vistas, también:

- Consultaba directamente las colecciones estáticas de `DatosApp`.
- Filtraba las citas por paciente.
- Buscaba citas por identificador.
- Cargaba las listas de pacientes y médicos.
- Construía copias de las citas agregando sus propiedades de navegación.

El método privado `ConNavegacion` concentraba lógica de consulta y transformación dentro del controlador. Esto producía un acoplamiento directo entre la presentación y el origen de datos, dificultaba las pruebas y obligaba a modificar el controlador si cambiaba la manera de obtener las citas.

Flujo anterior:

```text
CitaController ───────────────> DatosApp
       └── ConNavegacion()      colecciones estáticas y JSON
```

## Refactorización realizada

### 1. Extract Class

Extraje la lógica de consulta y composición de citas a la clase `CitaConsulta`. Esta clase se encarga de:

- Obtener todas las citas.
- Obtener las citas correspondientes a un paciente.
- Buscar una cita por su identificador.
- Proporcionar las listas de pacientes y médicos.
- Agregar los objetos `Paciente` y `Medico` a cada cita para mostrarlos en las vistas.

```csharp
public class CitaConsulta : ICitaConsulta
{
    public IReadOnlyCollection<Cita> ObtenerTodas() =>
        AgregarNavegacion(DatosApp.Citas);

    public IReadOnlyCollection<Cita> ObtenerPorPaciente(int pacienteId) =>
        AgregarNavegacion(DatosApp.Citas.Where(
            cita => cita.PacienteId == pacienteId));

    public Cita? ObtenerPorId(int id) =>
        DatosApp.Citas.FirstOrDefault(cita => cita.Id == id);
}
```

Con esta extracción, `CitaController` deja de conocer cómo se consultan y transforman los datos.

### 2. Creación de la abstracción

Creé la interfaz `ICitaConsulta` para definir el contrato que necesita el controlador:

```csharp
public interface ICitaConsulta
{
    IReadOnlyCollection<Cita> ObtenerTodas();
    IReadOnlyCollection<Cita> ObtenerPorPaciente(int pacienteId);
    Cita? ObtenerPorId(int id);
    IReadOnlyCollection<Paciente> ObtenerPacientes();
    IReadOnlyCollection<Medico> ObtenerMedicos();
}
```

Se utilizan colecciones de solo lectura en el contrato porque el consumidor necesita consultar los resultados, no modificar directamente su contenido.

### 3. Inyección de dependencias

Inyecté `ICitaConsulta` mediante el constructor primario de `CitaController`:

```csharp
public class CitaController(
    CitaServicio citaServicio,
    CitaService citaService,
    ICitaConsulta citaConsulta) : Controller
```

El controlador ahora delega las consultas:

```csharp
public IActionResult Index()
    => View(citaConsulta.ObtenerTodas());

public IActionResult PorPaciente(int pacienteId)
    => View(citaConsulta.ObtenerPorPaciente(pacienteId));
```

Finalmente registré la relación entre la abstracción y su implementación en el contenedor de ASP.NET Core:

```csharp
builder.Services.AddSingleton<ICitaConsulta, CitaConsulta>();
```

Flujo después de la refactorización:

```text
CitaController ──> ICitaConsulta <── CitaConsulta ──> DatosApp ──> JSON
   HTTP y vistas       contrato       consultas y composición
```

## Principios SOLID aplicados

### SRP — Single Responsibility Principle

El controlador se concentra en coordinar las solicitudes HTTP, validar el modelo y devolver una vista o redirección. `CitaConsulta` concentra la obtención y preparación de los datos de lectura.

### OCP — Open/Closed Principle

Es posible crear otra implementación de `ICitaConsulta`, por ejemplo una consulta basada en Entity Framework Core, sin cambiar las acciones del controlador.

### LSP — Liskov Substitution Principle

Cualquier implementación que respete el contrato `ICitaConsulta` puede sustituir a `CitaConsulta` y ser utilizada por `CitaController`.

### ISP — Interface Segregation Principle

La interfaz expone únicamente las operaciones de consulta que necesita el controlador de citas. No obliga a implementar acciones ajenas como autenticación o notificaciones.

### DIP — Dependency Inversion Principle

`CitaController`, como módulo de alto nivel, depende de `ICitaConsulta` y no de la clase concreta `CitaConsulta` ni directamente de `DatosApp`. La implementación concreta se decide en `Program.cs`, que funciona como raíz de composición.

## Resultado obtenido

La refactorización produjo los siguientes beneficios:

- Menor acoplamiento entre el controlador y el almacenamiento.
- Responsabilidades más claras y clases más pequeñas.
- Código más sencillo de mantener y extender.
- Posibilidad de sustituir la fuente de consultas.
- Mejor capacidad para crear pruebas unitarias usando una implementación falsa de `ICitaConsulta`.
- Conservación del comportamiento existente de las vistas y acciones MVC.

## Arquitectura del proyecto

| Proyecto | Responsabilidad |
|---|---|
| `CitasApp.Domain` | Modelos del dominio e interfaces principales. |
| `CitasApp.Application` | Servicios y casos de uso, como autenticación y confirmación de citas. |
| `CitasApp.Infrastructure` | Repositorios JSON, seguridad, observadores y otros adaptadores. |
| `CitasApp.Web` | Aplicación MVC, controladores, vistas, servicios web y configuración de DI. |
| `CitasApp.Api` | Endpoints REST para pacientes, médicos y citas. |

La documentación técnica complementaria y los diagramas se encuentran en [`docs/DIAGRAMAS.md`](docs/DIAGRAMAS.md).

## Patrones utilizados

- **Factory:** `RepositoryFactory` selecciona la implementación del repositorio de pacientes.
- **Decorator:** `LoggingPacienteRepository` agrega registro de operaciones sin modificar el repositorio decorado.
- **Observer:** `CitaService` notifica a `SmsObserver` y `EmailObserver` cuando una cita se confirma.
- **Dependency Injection:** las implementaciones se configuran en `Program.cs` y se entregan a sus consumidores.
- **Extract Class:** `CitaConsulta` recibe la lógica de consulta que antes estaba dentro de `CitaController`.

## Tecnologías

- .NET 10
- ASP.NET Core MVC
- ASP.NET Core Web API
- Razor Views
- Bootstrap
- System.Text.Json
- Autenticación por cookies
- BCrypt para contraseñas

## Estructura relevante

```text
CitasApp/
├── CitasApp.Domain/
│   ├── Interfaces/
│   └── Models/
├── CitasApp.Application/
│   └── Services/
├── CitasApp.Infrastructure/
│   ├── Observers/
│   ├── Repositories/
│   └── Security/
├── CitasApp.Web/
│   ├── Controllers/
│   │   └── CitaController.cs
│   ├── Data/
│   ├── Services/
│   │   ├── CitaConsulta.cs
│   │   ├── ICitaConsulta.cs
│   │   └── CitaServicio.cs
│   ├── Views/
│   └── Program.cs
├── CitasApp.Api/
└── docs/
    └── DIAGRAMAS.md
```

## Ejecución

### Requisitos

- SDK de .NET 10.

### Aplicación web

```bash
dotnet restore CitasApp.sln
dotnet run --project CitasApp.Web/CitasApp.Web.csproj
```

### API

```bash
dotnet run --project CitasApp.Api/CitasApp.Api.csproj
```

Al iniciar la aplicación web se crean datos de ejemplo si los archivos JSON están vacíos. También se registra de forma idempotente el usuario de demostración:

```text
Correo: admin@citasapp.com
Contraseña: Admin123
```

## Archivos modificados durante la refactorización

- `CitasApp.Web/Controllers/CitaController.cs`: delegación de consultas mediante DI.
- `CitasApp.Web/Services/ICitaConsulta.cs`: contrato de consulta.
- `CitasApp.Web/Services/CitaConsulta.cs`: clase extraída con la lógica de lectura y composición.
- `CitasApp.Web/Program.cs`: registro de `ICitaConsulta` y `CitaConsulta`.

La refactorización quedó registrada en el commit:

```text
8c8542b despues de aplicar refactorizacion, extract Class, DI
```
