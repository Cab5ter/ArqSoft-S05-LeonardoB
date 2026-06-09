using CitasApp.Models;

namespace CitasApp.Data;

public static class DatosApp
{
    public static List<Paciente> Pacientes = [];
    public static List<Medico> Medicos = [];
    public static List<Cita> Citas = [];

    public static void Inicializar(string contentRootPath)
    {
        JsonDataService.Initialize(contentRootPath);

        Pacientes = JsonDataService.CargarPacientes();
        Medicos = JsonDataService.CargarMedicos();
        Citas = JsonDataService.CargarCitas();

        if (Pacientes.Count == 0 && Medicos.Count == 0 && Citas.Count == 0)
        {
            SeedData();
            GuardarTodo();
        }
    }

    public static void GuardarPacientes() => JsonDataService.GuardarPacientes(Pacientes);
    public static void GuardarMedicos() => JsonDataService.GuardarMedicos(Medicos);
    public static void GuardarCitas() => JsonDataService.GuardarCitas(Citas);

    public static void GuardarTodo()
    {
        GuardarPacientes();
        GuardarMedicos();
        GuardarCitas();
    }

    public static int SiguienteIdPaciente() => Pacientes.Count == 0 ? 1 : Pacientes.Max(p => p.Id) + 1;
    public static int SiguienteIdMedico() => Medicos.Count == 0 ? 1 : Medicos.Max(m => m.Id) + 1;
    public static int SiguienteIdCita() => Citas.Count == 0 ? 1 : Citas.Max(c => c.Id) + 1;

    private static void SeedData()
    {
        Pacientes =
        [
            new() { Id = 1, Nombre = "Ana", Apellido = "García", Email = "ana@mail.com", Telefono = "555-0001" },
            new() { Id = 2, Nombre = "Luis", Apellido = "Martínez", Email = "luis@mail.com", Telefono = "555-0002" },
            new() { Id = 3, Nombre = "María", Apellido = "López", Email = "maria@mail.com", Telefono = "555-0003" }
        ];

        Medicos =
        [
            new() { Id = 1, Nombre = "Dr. Carlos", Apellido = "Reyes", Especialidad = "Medicina General", NumeroLicencia = "MG-10421" },
            new() { Id = 2, Nombre = "Dra. Patricia", Apellido = "Vega", Especialidad = "Pediatría", NumeroLicencia = "PD-20835" },
            new() { Id = 3, Nombre = "Dr. Roberto", Apellido = "Sánchez", Especialidad = "Cardiología", NumeroLicencia = "CA-30117" }
        ];

        Citas =
        [
            new() { Id = 1, PacienteId = 1, MedicoId = 1, Fecha = new DateOnly(2026, 6, 1), Hora = new TimeOnly(9, 0), Motivo = "Consulta general", Estado = EstadoCita.Confirmada },
            new() { Id = 2, PacienteId = 2, MedicoId = 2, Fecha = new DateOnly(2026, 6, 1), Hora = new TimeOnly(10, 0), Motivo = "Revisión de resultados", Estado = EstadoCita.Pendiente },
            new() { Id = 3, PacienteId = 3, MedicoId = 1, Fecha = new DateOnly(2026, 6, 3), Hora = new TimeOnly(11, 0), Motivo = "Primera consulta", Estado = EstadoCita.Pendiente }
        ];
    }
}
