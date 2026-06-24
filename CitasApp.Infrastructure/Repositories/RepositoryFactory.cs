using CitasApp.Interfaces;
using CitasApp.Repositories;

namespace CitasApp.Infrastructure.Repositories
{
    public static class RepositoryFactory
    {
        public static IPacienteRepository CrearPacienteRepository(string entorno, string dataPath)
        {
            return entorno switch
            {
                "Production" => new MemoriaPacienteRepository(),
                _            => new JsonPacienteRepository(dataPath)
            };
        }
    }
}
