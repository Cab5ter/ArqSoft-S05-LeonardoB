using CitasApp.Models;

namespace CitasApp.Interfaces
{
    public interface ICitaRepository
    {
        List<Cita> ObtenerTodos();
        List<Cita> ObtenerPorPaciente(int pacienteId);
    }
}
