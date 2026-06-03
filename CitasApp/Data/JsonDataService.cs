using System.Text.Json;
using CitasApp.Models;

namespace CitasApp.Data;

public static class JsonDataService
{
    private static string _basePath = "";
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static void Initialize(string contentRootPath)
    {
        _basePath = Path.Combine(contentRootPath, "Data", "json");
        Directory.CreateDirectory(_basePath);
    }

    public static List<Paciente> CargarPacientes()
    {
        var path = Path.Combine(_basePath, "pacientes.json");
        if (!File.Exists(path)) return [];
        return JsonSerializer.Deserialize<List<Paciente>>(File.ReadAllText(path), Options) ?? [];
    }

    public static void GuardarPacientes(List<Paciente> lista)
        => File.WriteAllText(Path.Combine(_basePath, "pacientes.json"),
            JsonSerializer.Serialize(lista, Options));

    public static List<Medico> CargarMedicos()
    {
        var path = Path.Combine(_basePath, "medicos.json");
        if (!File.Exists(path)) return [];
        return JsonSerializer.Deserialize<List<Medico>>(File.ReadAllText(path), Options) ?? [];
    }

    public static void GuardarMedicos(List<Medico> lista)
        => File.WriteAllText(Path.Combine(_basePath, "medicos.json"),
            JsonSerializer.Serialize(lista, Options));

    public static List<Cita> CargarCitas()
    {
        var path = Path.Combine(_basePath, "citas.json");
        if (!File.Exists(path)) return [];
        var registros = JsonSerializer.Deserialize<List<CitaRegistro>>(File.ReadAllText(path), Options) ?? [];
        return registros.Select(r => new Cita
        {
            Id = r.Id,
            PacienteId = r.PacienteId,
            MedicoId = r.MedicoId,
            Fecha = DateOnly.Parse(r.Fecha),
            Hora = TimeOnly.Parse(r.Hora),
            Motivo = r.Motivo,
            Estado = r.Estado
        }).ToList();
    }

    public static void GuardarCitas(List<Cita> lista)
    {
        var registros = lista.Select(c => new CitaRegistro
        {
            Id = c.Id,
            PacienteId = c.PacienteId,
            MedicoId = c.MedicoId,
            Fecha = c.Fecha.ToString("yyyy-MM-dd"),
            Hora = c.Hora.ToString("HH:mm"),
            Motivo = c.Motivo,
            Estado = c.Estado
        }).ToList();
        File.WriteAllText(Path.Combine(_basePath, "citas.json"),
            JsonSerializer.Serialize(registros, Options));
    }

    private sealed class CitaRegistro
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public string Fecha { get; set; } = "";
        public string Hora { get; set; } = "";
        public string Motivo { get; set; } = "";
        public EstadoCita Estado { get; set; }
    }
}
