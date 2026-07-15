using CitasApp.Data;
using CitasApp.Models;

namespace CitasApp.Services;

public class CitaConsulta : ICitaConsulta
{
    public IReadOnlyCollection<Cita> ObtenerTodas() =>
        AgregarNavegacion(DatosApp.Citas);

    public IReadOnlyCollection<Cita> ObtenerPorPaciente(int pacienteId) =>
        AgregarNavegacion(DatosApp.Citas.Where(c => c.PacienteId == pacienteId));

    public Cita? ObtenerPorId(int id) =>
        DatosApp.Citas.FirstOrDefault(c => c.Id == id);

    public IReadOnlyCollection<Paciente> ObtenerPacientes() => DatosApp.Pacientes;

    public IReadOnlyCollection<Medico> ObtenerMedicos() => DatosApp.Medicos;

    private static List<Cita> AgregarNavegacion(IEnumerable<Cita> citas) =>
        citas.Select(c => new Cita
        {
            Id = c.Id,
            PacienteId = c.PacienteId,
            MedicoId = c.MedicoId,
            Fecha = c.Fecha,
            Hora = c.Hora,
            Motivo = c.Motivo,
            Estado = c.Estado,
            Paciente = DatosApp.Pacientes.FirstOrDefault(p => p.Id == c.PacienteId),
            Medico = DatosApp.Medicos.FirstOrDefault(m => m.Id == c.MedicoId)
        }).ToList();
}
