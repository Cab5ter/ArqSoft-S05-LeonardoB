# CitasApp — Modelo C4 y documentación técnica

Este documento describe **CitasApp** siguiendo el **modelo C4** (Simon Brown): una jerarquía de
cuatro niveles de abstracción —**Contexto → Contenedores → Componentes → Código**— que permite
"hacer zoom" desde la visión general del sistema hasta las clases concretas.

Todos los diagramas están escritos en **Mermaid** (`C4Context`, `C4Container`, `C4Component` y
`classDiagram`) y reflejan el estado real del repositorio, no un modelo genérico.

> CitasApp es una solución .NET 10 multi‑proyecto (arquitectura hexagonal) para la gestión de
> citas médicas, con dos *hosts* que reutilizan el mismo núcleo (**Web MVC** y **API REST**),
> persistencia en archivos JSON y tres patrones GoF: **Factory**, **Decorator** y **Observer**.

| Nivel C4 | Pregunta que responde | Diagrama |
|---|---|---|
| **1 · Contexto** | ¿Quién usa el sistema y con qué interactúa? | [§1](#1-nivel-1--contexto-del-sistema) |
| **2 · Contenedores** | ¿De qué unidades desplegables/ejecutables se compone? | [§2](#2-nivel-2--contenedores) |
| **3 · Componentes** | ¿Qué componentes hay dentro de un contenedor y cómo colaboran? | [§3](#3-nivel-3--componentes) |
| **4 · Código** | ¿Cómo se implementan esos componentes (clases)? | [§4](#4-nivel-4--código-clases) |

---

## 1. Nivel 1 — Contexto del sistema

Visión de más alto nivel: los actores humanos, el sistema **CitasApp** como una caja negra, y los
sistemas externos con los que se comunica. Las notificaciones **SMS/Email** hoy están *simuladas*
(escriben en consola), pero se modelan como sistemas externos porque representan un canal fuera
del sistema.

```mermaid
C4Context
    title Nivel 1 - Contexto del sistema (CitasApp)

    Person(usuario, "Usuario / Recepcionista", "Gestiona pacientes, medicos y citas desde el navegador")
    Person(consumidor, "Consumidor de API", "Aplicaciones o clientes que consultan datos via REST")

    System(citasapp, "CitasApp", "Sistema de gestion de citas medicas: interfaz web MVC + API REST")

    System_Ext(sms, "Canal SMS (simulado)", "Notificacion de confirmacion por SMS - salida por consola")
    System_Ext(email, "Canal Email (simulado)", "Notificacion de confirmacion por email - salida por consola")

    Rel(usuario, citasapp, "Administra citas, pacientes y medicos", "HTTPS")
    Rel(consumidor, citasapp, "Consulta pacientes, medicos y citas", "JSON / HTTPS")
    Rel(citasapp, sms, "Notifica al confirmar una cita")
    Rel(citasapp, email, "Notifica al confirmar una cita")

    UpdateLayoutConfig($c4ShapeInRow="2", $c4BoundaryInRow="1")
```

---

## 2. Nivel 2 — Contenedores

Se abre la caja negra de CitasApp. Cada **contenedor** es una unidad ejecutable o una librería
desplegable. Aquí se ven los dos *hosts* (Web y API) y las tres librerías del núcleo hexagonal,
más el almacén de datos. Las flechas "usa" reflejan las `ProjectReference` reales de cada
`.csproj`; nótese que todas apuntan hacia el dominio.

```mermaid
C4Container
    title Nivel 2 - Contenedores (CitasApp)

    Person(usuario, "Usuario / Recepcionista", "Navegador web")
    Person(consumidor, "Consumidor de API", "Cliente REST")

    System_Boundary(sb, "CitasApp") {
        Container(web, "CitasApp.Web", "ASP.NET Core MVC + Razor + Bootstrap 5", "Interfaz web: CRUD de pacientes, medicos y citas")
        Container(api, "CitasApp.Api", "ASP.NET Core Web API + Swagger", "API REST de solo lectura + calculadora de ejemplo")
        Container(app, "CitasApp.Application", "Libreria .NET 10", "Servicios / casos de uso: CitaService (Observer), PacienteService, MedicoService")
        Container(infra, "CitasApp.Infrastructure", "Libreria .NET 10", "Adaptadores: repos JSON y Memoria, RepositoryFactory, Decorator, Observers")
        Container(domain, "CitasApp.Domain", "Libreria .NET 10", "Nucleo: modelos e interfaces (puertos)")
        ContainerDb(json, "Almacen JSON", "System.Text.Json (archivos)", "pacientes.json, medicos.json, citas.json")
    }

    Rel(usuario, web, "Usa", "HTTPS")
    Rel(consumidor, api, "Consulta", "JSON / HTTPS")

    Rel(web, app, "Usa")
    Rel(web, infra, "Usa")
    Rel(web, domain, "Usa")
    Rel(api, app, "Usa")
    Rel(api, infra, "Usa")
    Rel(api, domain, "Usa")
    Rel(app, infra, "Usa")
    Rel(app, domain, "Usa")
    Rel(infra, domain, "Usa")

    Rel(web, json, "Carga y guarda datos (JsonDataService)", "File IO")
    Rel(infra, json, "Lee datos (repos JSON)", "File IO")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```

---

## 3. Nivel 3 — Componentes

Se abre un contenedor para mostrar sus **componentes** internos y cómo colaboran. Se eligen los dos
flujos que concentran los patrones de diseño: el **listado de pacientes** (Factory + Decorator) y
la **confirmación de una cita** (Observer). Participan componentes de `Web`, `Infrastructure` y
`Application`.

```mermaid
C4Component
    title Nivel 3 - Componentes (flujos de Paciente y confirmacion de Cita)

    Person(usuario, "Usuario", "Navegador")

    Container_Boundary(web, "CitasApp.Web") {
        Component(pacCtrl, "PacienteController", "MVC Controller", "CRUD de pacientes")
        Component(citaCtrl, "CitaController", "MVC Controller", "CRUD de citas y confirmacion")
        Component(citaServicio, "CitaServicio", "Clase", "Alta/edicion/baja de citas sobre DatosApp")
        Component(datos, "DatosApp / JsonDataService", "Clases estaticas", "Estado en memoria y carga/guardado en JSON")
    }

    Container_Boundary(app, "CitasApp.Application") {
        Component(citaSvc, "CitaService", "Sujeto (Observer)", "Mantiene la lista de observadores y notifica")
    }

    Container_Boundary(infra, "CitasApp.Infrastructure") {
        Component(factory, "RepositoryFactory", "Factory (estatico)", "Elige el repositorio segun el entorno")
        Component(logging, "LoggingPacienteRepository", "Decorator", "Registra en log antes/despues y delega")
        Component(jsonRepo, "JsonPacienteRepository", "Adaptador", "Lee pacientes.json")
        Component(memRepo, "MemoriaPacienteRepository", "Adaptador", "Datos en memoria (entorno Production)")
        Component(sms, "SmsObserver", "Observer concreto", "Simula SMS por consola")
        Component(email, "EmailObserver", "Observer concreto", "Simula email por consola")
    }

    ContainerDb(json, "pacientes.json", "JSON", "Persistencia de pacientes")

    Rel(usuario, pacCtrl, "GET /Paciente")
    Rel(pacCtrl, logging, "ObtenerTodos()", "IPacienteRepository")
    Rel(factory, logging, "crea (repo envuelto)")
    Rel(logging, jsonRepo, "delega")
    Rel(factory, jsonRepo, "crea (por defecto)")
    Rel(factory, memRepo, "crea (Production)")
    Rel(jsonRepo, json, "lee")

    Rel(usuario, citaCtrl, "POST /Cita/Editar (Confirmada)")
    Rel(citaCtrl, citaServicio, "Actualizar()")
    Rel(citaServicio, datos, "GuardarCitas()")
    Rel(citaCtrl, citaSvc, "Confirmar()")
    Rel(citaSvc, sms, "Notificar()", "ICitaObserver")
    Rel(citaSvc, email, "Notificar()", "ICitaObserver")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```

---

## 4. Nivel 4 — Código (clases)

El nivel más detallado del C4 se expresa como diagramas de clases. Se dividen en tres vistas para
mantener la legibilidad: el **modelo de dominio**, los **puertos y adaptadores**, y las clases que
materializan cada **patrón GoF**.

### 4.1 Modelo de dominio

`Cita` guarda las claves foráneas (`PacienteId`, `MedicoId`) y, además, propiedades de navegación
opcionales que la capa web rellena para las vistas. `CitaJson` es el DTO usado para serializar
fechas/horas como texto.

```mermaid
classDiagram
    class Paciente {
        +int Id
        +string Nombre
        +string Apellido
        +string Email
        +string Telefono
    }
    class Medico {
        +int Id
        +string Nombre
        +string Apellido
        +string Especialidad
        +string NumeroLicencia
        +string NombreCompleto()
    }
    class Cita {
        +int Id
        +int PacienteId
        +int MedicoId
        +DateOnly Fecha
        +TimeOnly Hora
        +string Motivo
        +EstadoCita Estado
        +Paciente Paciente
        +Medico Medico
    }
    class CitaJson {
        +int Id
        +int PacienteId
        +int MedicoId
        +string Fecha
        +string Hora
        +string Motivo
        +EstadoCita Estado
    }
    class EstadoCita {
        <<enumeration>>
        Pendiente
        Confirmada
        Cancelada
    }

    Cita "1" --> "0..1" Paciente : navegacion
    Cita "1" --> "0..1" Medico : navegacion
    Cita --> EstadoCita : Estado
    Cita ..> CitaJson : se serializa como
```

### 4.2 Puertos (Domain) y adaptadores (Infrastructure)

```mermaid
classDiagram
    class IPacienteRepository {
        <<interface>>
        +List~Paciente~ ObtenerTodos()
        +Paciente ObtenerPorId(int id)
    }
    class IMedicoRepository {
        <<interface>>
        +List~Medico~ ObtenerTodos()
        +Medico ObtenerPorId(int id)
    }
    class ICitaRepository {
        <<interface>>
        +List~Cita~ ObtenerTodos()
        +List~Cita~ ObtenerPorPaciente(int pacienteId)
    }
    class ICitaObserver {
        <<interface>>
        +void Notificar(Cita cita, string evento)
    }

    class JsonPacienteRepository
    class JsonMedicoRepository
    class JsonCitaRepository
    class MemoriaPacienteRepository
    class LoggingPacienteRepository
    class SmsObserver
    class EmailObserver

    IPacienteRepository <|.. JsonPacienteRepository
    IPacienteRepository <|.. MemoriaPacienteRepository
    IPacienteRepository <|.. LoggingPacienteRepository
    IMedicoRepository <|.. JsonMedicoRepository
    ICitaRepository <|.. JsonCitaRepository
    ICitaObserver <|.. SmsObserver
    ICitaObserver <|.. EmailObserver
```

### 4.3 Patrones GoF

**Factory** (`RepositoryFactory`) decide qué implementación crear según el entorno;
**Decorator** (`LoggingPacienteRepository`) envuelve a otro `IPacienteRepository` añadiendo logging
sin modificarlo (Open/Closed); **Observer** (`CitaService` + `SmsObserver`/`EmailObserver`) notifica
a los suscriptores al confirmar una cita.

```mermaid
classDiagram
    class IPacienteRepository {
        <<interface>>
    }
    class RepositoryFactory {
        <<static>>
        +CrearPacienteRepository(string entorno, string dataPath) IPacienteRepository
    }
    class LoggingPacienteRepository {
        -IPacienteRepository _inner
        +ObtenerTodos()
        +ObtenerPorId(int id)
    }
    class JsonPacienteRepository
    class MemoriaPacienteRepository

    class CitaService {
        -List~ICitaObserver~ _observadores
        +Suscribir(ICitaObserver observador)
        +Confirmar(Cita cita)
    }
    class ICitaObserver {
        <<interface>>
        +Notificar(Cita cita, string evento)
    }
    class SmsObserver
    class EmailObserver

    RepositoryFactory ..> IPacienteRepository : Factory - crea
    IPacienteRepository <|.. LoggingPacienteRepository
    IPacienteRepository <|.. JsonPacienteRepository
    IPacienteRepository <|.. MemoriaPacienteRepository
    LoggingPacienteRepository o--> IPacienteRepository : Decorator - _inner

    CitaService o--> ICitaObserver : Observer - notifica
    ICitaObserver <|.. SmsObserver
    ICitaObserver <|.. EmailObserver
```

---

## 5. Vistas dinámicas (secuencia)

Complementan el nivel de Componentes mostrando el orden temporal de las interacciones.

### 5.1 Listar pacientes — Factory + Decorator (Web)

```mermaid
sequenceDiagram
    actor U as Usuario
    participant PC as PacienteController
    participant Log as LoggingPacienteRepository
    participant Json as JsonPacienteRepository
    participant FS as pacientes.json

    U->>PC: GET /Paciente
    PC->>Log: ObtenerTodos()
    Log-->>Log: log "ObtenerTodos - inicio"
    Log->>Json: ObtenerTodos()
    Json->>FS: ReadAllText + Deserialize
    FS-->>Json: List~Paciente~
    Json-->>Log: List~Paciente~
    Log-->>Log: log "ObtenerTodos - N registros"
    Log-->>PC: List~Paciente~
    PC-->>U: Vista Index (tabla de pacientes)
```

### 5.2 Confirmar una cita — Observer (Web)

```mermaid
sequenceDiagram
    actor U as Usuario
    participant CC as CitaController
    participant CS as CitaServicio
    participant DA as DatosApp
    participant Svc as CitaService (sujeto)
    participant Sms as SmsObserver
    participant Email as EmailObserver

    U->>CC: POST /Cita/Editar (Estado=Confirmada)
    CC->>CS: Actualizar(cita)
    CS->>DA: GuardarCitas() -> citas.json
    alt cita.Estado == Confirmada
        CC->>Svc: Confirmar(cita)
        Svc->>Sms: Notificar(cita, "confirmada")
        Sms-->>Svc: log [SMS] en consola
        Svc->>Email: Notificar(cita, "confirmada")
        Email-->>Svc: log [EMAIL] en consola
    end
    CC-->>U: RedirectToAction(Index)
```

---

## 6. Detalle de implementación

### 6.1 Inyección de dependencias

**`CitasApp.Web/Program.cs`**
- `IPacienteRepository` → **Scoped**: `RepositoryFactory.CrearPacienteRepository(entorno, dataPath)`
  envuelto en `LoggingPacienteRepository` (Factory + Decorator).
- `CitaService` → **Singleton**: se le suscriben `SmsObserver` y `EmailObserver`.
- `CitaServicio` → **Singleton**: CRUD de citas sobre `DatosApp`.
- `DatosApp.Inicializar(...)` carga los JSON y siembra datos de ejemplo si están vacíos.

**`CitasApp.Api/Program.cs`**
- Repositorios JSON registrados directamente contra sus interfaces (**Scoped**).
- Servicios `PacienteService`, `MedicoService`, `CitaService` (**Scoped**).
- Swagger/OpenAPI bajo la ruta `/docs`.

### 6.2 Endpoints

**Web MVC**

| Controlador | Acciones |
|---|---|
| `HomeController` | `Index`, `Privacy`, `Error` |
| `PacienteController` | `Index`, `Detalle`, `Crear`, `Editar`, `Eliminar` |
| `MedicoController` | `Index`, `Detalle`, `Crear`, `Editar`, `Eliminar` |
| `CitaController` | `Index`, `PorPaciente`, `Crear`, `Editar`, `Eliminar` |

**API REST**

| Ruta | Método | Descripción |
|---|---|---|
| `/api/Pacientes` · `/api/Pacientes/{id}` | GET | Pacientes |
| `/api/Medicos` · `/api/Medicos/{id}` | GET | Médicos |
| `/api/Citas` · `/api/Citas/porpaciente/{pacienteId}` | GET | Citas |
| `/api/Calculadora/{sumar\|restar\|multiplicar\|dividir}` | GET | Aritmética de ejemplo |

### 6.3 Tecnologías

| Tecnología | Uso |
|---|---|
| **.NET 10 / ASP.NET Core** | Framework (MVC y Web API) |
| **C# 13** | Lenguaje (`ImplicitUsings`, `Nullable`, colecciones `[]`) |
| **Razor + Bootstrap 5** | Vistas del lado del servidor |
| **System.Text.Json** | Serialización de la persistencia |
| **Swashbuckle.AspNetCore 7.3.1** | Swagger/OpenAPI en `CitasApp.Api` |
| **JSON (archivos)** | Persistencia por defecto (`Data/json/`) |
