using CitasApp.Interfaces;
using CitasApp.Models;
using System.Text.Json;

namespace CitasApp.Infrastructure.Repositories
{
    // Adaptador de salida: implementa el puerto IUsuarioRepository sobre un JSON,
    // siguiendo el mismo patrón que JsonPacienteRepository.
    public class JsonUsuarioRepository : IUsuarioRepository
    {
        private readonly string _path;
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public JsonUsuarioRepository(string dataPath)
        {
            _path = Path.Combine(dataPath, "usuarios.json");
        }

        public Usuario? ObtenerPorEmail(string email) =>
            ObtenerTodos().FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        public void Agregar(Usuario usuario)
        {
            var usuarios = ObtenerTodos();
            usuario.Id = usuarios.Count == 0 ? 1 : usuarios.Max(u => u.Id) + 1;
            usuarios.Add(usuario);
            Guardar(usuarios);
        }

        private List<Usuario> ObtenerTodos()
        {
            if (!File.Exists(_path)) return new();
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Usuario>>(json, _options) ?? new();
        }

        private void Guardar(List<Usuario> usuarios)
        {
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(_path, JsonSerializer.Serialize(usuarios, _options));
        }
    }
}
