using CitasApp.Interfaces;
using CitasApp.Models;
using System.Text.Json;

namespace CitasApp.Repositories
{
    public class JsonMedicoRepository : IMedicoRepository
    {
        private readonly string _path;
        private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

        public JsonMedicoRepository(string dataPath)
        {
            _path = Path.Combine(dataPath, "medicos.json");
        }

        public List<Medico> ObtenerTodos()
        {
            if (!File.Exists(_path)) return new();
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Medico>>(json, _options) ?? new();
        }

        public Medico? ObtenerPorId(int id) =>
            ObtenerTodos().FirstOrDefault(m => m.Id == id);
    }
}
