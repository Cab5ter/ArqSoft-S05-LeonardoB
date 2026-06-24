using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Infrastructure.Repositories
{
    public class MemoriaPacienteRepository : IPacienteRepository
    {
        private readonly List<Paciente> _pacientes =
        [
            new() { Id = 1, Nombre = "Ana",   Apellido = "García",   Email = "ana@mail.com",   Telefono = "555-0001" },
            new() { Id = 2, Nombre = "Luis",  Apellido = "Martínez", Email = "luis@mail.com",  Telefono = "555-0002" },
            new() { Id = 3, Nombre = "María", Apellido = "López",    Email = "maria@mail.com", Telefono = "555-0003" }
        ];

        public List<Paciente> ObtenerTodos() => _pacientes;

        public Paciente? ObtenerPorId(int id) => _pacientes.FirstOrDefault(p => p.Id == id);
    }
}
