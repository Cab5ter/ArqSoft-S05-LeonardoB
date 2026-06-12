# CitasApp — Sistema de Citas Médicas

Aplicación web MVC para la gestión de citas médicas. Permite administrar pacientes, médicos y citas con persistencia de datos en archivos JSON, organizada en una **arquitectura hexagonal multi-proyecto**.

## Descripción

CitasApp es un sistema que permite:

- Registrar y administrar **pacientes** con su información de contacto.
- Gestionar el directorio de **médicos** y sus especialidades.
- Programar **citas** médicas asignando paciente, médico, fecha, hora y motivo.
- Ver el estado de cada cita (Pendiente, Confirmada, Cancelada).
- Persistencia total: los datos se guardan en archivos JSON y se mantienen entre reinicios de la aplicación.

## Qué se hizo

Se desarrolló la aplicación con **arquitectura hexagonal** (puertos y adaptadores), separando la solución en tres proyectos con responsabilidades claras:

| Proyecto | Capa | Contenido |
|---|---|---|
| **CitasApp.Domain** | Dominio (núcleo) | Modelos (`Paciente`, `Medico`, `Cita`, `CitaJson`, `EstadoCita`) y puertos: interfaces de repositorio (`IPacienteRepository`, `IMedicoRepository`, `ICitaRepository`). No depende de nada. |
| **CitasApp.Infrastructure** | Infraestructura (adaptadores) | Implementaciones de los repositorios sobre archivos JSON (`JsonPacienteRepository`, `JsonMedicoRepository`, `JsonCitaRepository`). Depende solo de Domain. |
| **CitasApp.Web** | Presentación | Controladores MVC, vistas Razor, `Program.cs` e inicialización de datos (`DatosApp`, `JsonDataService`). Depende de Domain e Infrastructure. |

Con esto, el dominio queda aislado de los detalles de persistencia: la capa web consume las abstracciones del dominio y la infraestructura provee los adaptadores concretos, de modo que el almacenamiento JSON podría reemplazarse (por ejemplo, por una base de datos) sin tocar el núcleo de la aplicación.

Sobre esta base se implementó:

- Modelos de dominio: `Paciente`, `Medico`, `Cita` y el enum `EstadoCita`.
- Controladores MVC con acciones de listado, detalle, creación y edición para cada entidad.
- Vistas Razor con Bootstrap y navegación global mediante navbar.
- Persistencia en archivos JSON con `System.Text.Json`, con datos semilla (*seed*) cuando no existen archivos previos.

## Estructura de la solución

```
CitasApp.sln
├── CitasApp.Domain/          # Modelos e interfaces (puertos)
│   ├── Models/
│   └── Interfaces/
├── CitasApp.Infrastructure/  # Repositorios JSON (adaptadores)
│   └── Repositories/
└── CitasApp.Web/             # Aplicación MVC (presentación)
    ├── Controllers/
    ├── Views/
    └── Data/
```

## Tecnologías

| Tecnología | Descripción |
|---|---|
| **ASP.NET Core 10** | Framework principal — patrón MVC |
| **C# 13** | Lenguaje de programación |
| **Razor Views** | Motor de vistas del lado del servidor |
| **Bootstrap 5** | Estilos y componentes visuales |
| **System.Text.Json** | Serialización/deserialización de datos JSON |
| **JSON (archivos)** | Capa de persistencia (`Data/json/`) |

## Capturas de pantalla

### Inicio
![Inicio](screenshots/home.png)

### Pacientes
![Pacientes](screenshots/pacientes.png)

### Médicos
![Médicos](screenshots/medicos.png)

### Citas
![Citas](screenshots/citas.png)

---

> **Nota:** Se utilizó IA (Claude) como apoyo durante el desarrollo, principalmente para corregir errores de compilación y resolver dudas técnicas puntuales.
