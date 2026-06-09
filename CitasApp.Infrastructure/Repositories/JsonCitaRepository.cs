using CitasApp.Interfaces;
using CitasApp.Models;
using System.Text.Json;

namespace CitasApp.Repositories
{
    public class JsonCitaRepository : ICitaRepository
    {
        private readonly string _path;
        private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

        public JsonCitaRepository(string dataPath)
        {
            _path = Path.Combine(dataPath, "citas.json");
        }

        public List<Cita> ObtenerTodos()
        {
            if (!File.Exists(_path)) return new();
            var json = File.ReadAllText(_path);
            var citasJson = JsonSerializer.Deserialize<List<CitaJson>>(json, _options) ?? new();
            return citasJson.Select(c => new Cita
            {
                Id = c.Id,
                PacienteId = c.PacienteId,
                MedicoId = c.MedicoId,
                Fecha = DateOnly.Parse(c.Fecha),
                Hora = TimeOnly.Parse(c.Hora),
                Motivo = c.Motivo,
                Estado = c.Estado
            }).ToList();
        }

        public List<Cita> ObtenerPorPaciente(int pacienteId) =>
            ObtenerTodos().Where(c => c.PacienteId == pacienteId).ToList();
    }
}
