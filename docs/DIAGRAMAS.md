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

> Los diagramas C4 de este documento se representan con `flowchart` estilizado (no con la sintaxis
> `C4*` experimental de Mermaid) para lograr un trazado limpio; el modelo C4 es independiente de la
> notación empleada.

```mermaid
flowchart TB
    usuario["Usuario / Recepcionista<br/><b>[Persona]</b><br/>Gestiona pacientes, medicos y citas"]
    consumidor["Consumidor de API<br/><b>[Persona]</b><br/>Consulta datos via REST"]
    citasapp["CitasApp<br/><b>[Sistema]</b><br/>Gestion de citas medicas (Web MVC + API REST)"]
    sms["Canal SMS<br/><b>[Sistema externo - simulado]</b>"]
    email["Canal Email<br/><b>[Sistema externo - simulado]</b>"]

    usuario -->|"Administra (HTTPS)"| citasapp
    consumidor -->|"Consulta (REST)"| citasapp
    citasapp -->|"Notifica al confirmar cita"| sms
    citasapp -->|"Notifica al confirmar cita"| email

    classDef person fill:#08427b,stroke:#052e56,color:#fff
    classDef system fill:#1168bd,stroke:#0b4884,color:#fff
    classDef ext fill:#6b6b6b,stroke:#4d4d4d,color:#fff
    class usuario,consumidor person
    class citasapp system
    class sms,email ext
```

---

## 2. Nivel 2 — Contenedores

Se abre la caja negra de CitasApp. Cada **contenedor** es una unidad ejecutable o una librería
desplegable. Aquí se ven los dos *hosts* (Web y API) y las tres librerías del núcleo hexagonal,
más el almacén de datos. Las flechas "usa" reflejan las `ProjectReference` reales de cada
`.csproj`; nótese que todas apuntan hacia el dominio.

```mermaid
flowchart TB
    usuario["Usuario / Recepcionista<br/><b>[Persona]</b>"]
    consumidor["Consumidor de API<br/><b>[Persona]</b>"]

    subgraph sys["Sistema CitasApp"]
        direction TB
        web["CitasApp.Web<br/><b>[Contenedor]</b><br/>ASP.NET Core MVC + Razor + Bootstrap 5"]
        api["CitasApp.Api<br/><b>[Contenedor]</b><br/>ASP.NET Core Web API + Swagger"]
        subgraph core["Nucleo hexagonal (librerias .NET 10)"]
            direction TB
            app["CitasApp.Application<br/><b>[Contenedor]</b><br/>Servicios / casos de uso"]
            infra["CitasApp.Infrastructure<br/><b>[Contenedor]</b><br/>Adaptadores + patrones GoF"]
            domain["CitasApp.Domain<br/><b>[Contenedor]</b><br/>Modelos + interfaces (puertos)"]
            app --> infra --> domain
        end
        json[("Almacen JSON<br/><b>[Datos]</b><br/>pacientes / medicos / citas .json")]
    end

    usuario -->|"Usa (HTTPS)"| web
    consumidor -->|"Consulta (REST)"| api
    web --> core
    api --> core
    web -->|"JsonDataService"| json
    infra -->|"repos JSON"| json

    classDef person fill:#08427b,stroke:#052e56,color:#fff
    classDef container fill:#1168bd,stroke:#0b4884,color:#fff
    classDef db fill:#2e7d32,stroke:#1b5e20,color:#fff
    class usuario,consumidor person
    class web,api,app,infra,domain container
    class json db
```

---

## 3. Nivel 3 — Componentes

Se abre un contenedor para mostrar sus **componentes** internos y cómo colaboran. Se eligen los dos
flujos que concentran los patrones de diseño: el **listado de pacientes** (Factory + Decorator) y
la **confirmación de una cita** (Observer). Participan componentes de `Web`, `Infrastructure` y
`Application`.

```mermaid
flowchart TB
    usuario["Usuario<br/><b>[Persona]</b>"]

    subgraph web["CitasApp.Web [Contenedor]"]
        pacCtrl["PacienteController<br/><b>[Componente]</b>"]
        citaCtrl["CitaController<br/><b>[Componente]</b>"]
        citaServicio["CitaServicio<br/><b>[Componente]</b>"]
        datos["DatosApp / JsonDataService<br/><b>[Componente]</b>"]
    end

    subgraph app["CitasApp.Application [Contenedor]"]
        citaSvc["CitaService<br/><b>[Sujeto - Observer]</b>"]
    end

    subgraph infra["CitasApp.Infrastructure [Contenedor]"]
        factory["RepositoryFactory<br/><b>[Factory]</b>"]
        logging["LoggingPacienteRepository<br/><b>[Decorator]</b>"]
        jsonRepo["JsonPacienteRepository<br/><b>[Adaptador]</b>"]
        memRepo["MemoriaPacienteRepository<br/><b>[Adaptador]</b>"]
        sms["SmsObserver<br/><b>[Observer concreto]</b>"]
        email["EmailObserver<br/><b>[Observer concreto]</b>"]
    end

    json[("pacientes.json<br/><b>[Datos]</b>")]

    usuario -->|"GET /Paciente"| pacCtrl
    pacCtrl -->|"ObtenerTodos() : IPacienteRepository"| logging
    factory -.->|"crea (envuelto)"| logging
    logging -->|"delega"| jsonRepo
    factory -.->|"crea (por defecto)"| jsonRepo
    factory -.->|"crea (Production)"| memRepo
    jsonRepo -->|"lee"| json

    usuario -->|"POST /Cita/Editar (Confirmada)"| citaCtrl
    citaCtrl -->|"Actualizar()"| citaServicio
    citaServicio -->|"GuardarCitas()"| datos
    citaCtrl -->|"Confirmar()"| citaSvc
    citaSvc -->|"Notificar()"| sms
    citaSvc -->|"Notificar()"| email

    classDef person fill:#08427b,stroke:#052e56,color:#fff
    classDef comp fill:#1168bd,stroke:#0b4884,color:#fff
    classDef db fill:#2e7d32,stroke:#1b5e20,color:#fff
    class usuario person
    class pacCtrl,citaCtrl,citaServicio,datos,citaSvc,factory,logging,jsonRepo,memRepo,sms,email comp
    class json db
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
