using CitasApp.Models;

namespace CitasApp.Services;

public interface ICitaConsulta
{
    IReadOnlyCollection<Cita> ObtenerTodas();
    IReadOnlyCollection<Cita> ObtenerPorPaciente(int pacienteId);
    Cita? ObtenerPorId(int id);
    IReadOnlyCollection<Paciente> ObtenerPacientes();
    IReadOnlyCollection<Medico> ObtenerMedicos();
}
