using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Application.Services;

public class MedicoService
{
    private readonly IMedicoRepository _repo;

    public MedicoService(IMedicoRepository repo)
    {
        _repo = repo;
    }

    public List<Medico> ObtenerTodos() => _repo.ObtenerTodos();
    public Medico? ObtenerPorId(int id) => _repo.ObtenerPorId(id);
}
