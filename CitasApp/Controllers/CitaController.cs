using CitasApp.Data;
using CitasApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers;

public class CitaController : Controller
{
    private static List<Cita> ResolverNavegacion(IEnumerable<Cita> citas) =>
        citas.Select(c => new Cita
        {
            Id = c.Id,
            PacienteId = c.PacienteId,
            MedicoId = c.MedicoId,
            Fecha = c.Fecha,
            Hora = c.Hora,
            Motivo = c.Motivo,
            Estado = c.Estado,
            Paciente = DatosApp.Pacientes.FirstOrDefault(p => p.Id == c.PacienteId),
            Medico = DatosApp.Medicos.FirstOrDefault(m => m.Id == c.MedicoId)
        }).ToList();

    public IActionResult Index()
    {
        return View(ResolverNavegacion(DatosApp.Citas));
    }

    public IActionResult PorPaciente(int pacienteId)
    {
        var filtradas = DatosApp.Citas.Where(c => c.PacienteId == pacienteId);
        return View(ResolverNavegacion(filtradas));
    }
}
