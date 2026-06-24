using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Application.Services;

public class PacienteService
{
    private readonly IPacienteRepository _repo;

    public PacienteService(IPacienteRepository repo)
    {
        _repo = repo;
    }

    public List<Paciente> ObtenerTodos() => _repo.ObtenerTodos();
    public Paciente? ObtenerPorId(int id) => _repo.ObtenerPorId(id);
}
