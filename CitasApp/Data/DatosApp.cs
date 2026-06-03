using CitasApp.Models;

namespace CitasApp.Data;

public static class DatosApp
{
    public static List<Paciente> Pacientes =
    [
        new() { Id = 1, Nombre = "Ana", Apellido = "García", Email = "ana@mail.com", Telefono = "555-0001" },
        new() { Id = 2, Nombre = "Luis", Apellido = "Martínez", Email = "luis@mail.com", Telefono = "555-0002" },
        new() { Id = 3, Nombre = "María", Apellido = "López", Email = "maria@mail.com", Telefono = "555-0003" }
    ];

    public static List<Medico> Medicos =
    [
        new() { Id = 1, Nombre = "Dr. Carlos", Apellido = "Reyes", Especialidad = "Medicina General", NumeroLicencia = "MG-10421" },
        new() { Id = 2, Nombre = "Dra. Patricia", Apellido = "Vega", Especialidad = "Pediatría", NumeroLicencia = "PD-20835" },
        new() { Id = 3, Nombre = "Dr. Roberto", Apellido = "Sánchez", Especialidad = "Cardiología", NumeroLicencia = "CA-30117" }
    ];

    public static List<Cita> Citas =
    [
        new() { Id = 1, PacienteId = 1, MedicoId = 1, Fecha = new DateOnly(2026, 6, 1), Hora = new TimeOnly(9, 0), Motivo = "Consulta general", Estado = EstadoCita.Confirmada },
        new() { Id = 2, PacienteId = 2, MedicoId = 2, Fecha = new DateOnly(2026, 6, 1), Hora = new TimeOnly(10, 0), Motivo = "Revisión de resultados", Estado = EstadoCita.Pendiente },
        new() { Id = 3, PacienteId = 3, MedicoId = 1, Fecha = new DateOnly(2026, 6, 3), Hora = new TimeOnly(11, 0), Motivo = "Primera consulta", Estado = EstadoCita.Pendiente }
    ];
}
