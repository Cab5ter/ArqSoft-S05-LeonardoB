using CitasApp.Models;

namespace CitasApp.Interfaces
{
    public interface IPacienteRepository
    {
        List<Paciente> ObtenerTodos();
        Paciente? ObtenerPorId(int id);
    }
}
