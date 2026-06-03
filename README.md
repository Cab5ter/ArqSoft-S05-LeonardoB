# CitasApp — Sistema de Citas Médicas

Aplicación web MVC para la gestión de citas médicas. Permite administrar pacientes, médicos y citas con persistencia de datos en archivos JSON.

## Descripción

CitasApp es un sistema que permite:

- Registrar y administrar **pacientes** con su información de contacto.
- Gestionar el directorio de **médicos** y sus especialidades.
- Programar **citas** médicas asignando paciente, médico, fecha, hora y motivo.
- Ver el estado de cada cita (Pendiente, Confirmada, Cancelada).
- Persistencia total: los datos se guardan en archivos JSON y se mantienen entre reinicios de la aplicación.

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
