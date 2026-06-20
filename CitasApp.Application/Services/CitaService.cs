using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Application.Services;

public class CitaService
{
    private readonly ICitaRepository _repo;

    public CitaService(ICitaRepository repo)
    {
        _repo = repo;
    }

    public List<Cita> ObtenerTodos() => _repo.ObtenerTodos();
    public List<Cita> ObtenerPorPaciente(int pacienteId) => _repo.ObtenerPorPaciente(pacienteId);
}
