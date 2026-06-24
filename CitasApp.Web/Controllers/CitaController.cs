using CitasApp.Application.Services;
using CitasApp.Data;
using CitasApp.Models;
using CitasApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers;

public class CitaController(CitaServicio citaServicio, CitaService citaService) : Controller
{
    private static List<Cita> ConNavegacion(IEnumerable<Cita> citas) =>
        citas.Select(c => new Cita
        {
            Id         = c.Id,
            PacienteId = c.PacienteId,
            MedicoId   = c.MedicoId,
            Fecha      = c.Fecha,
            Hora       = c.Hora,
            Motivo     = c.Motivo,
            Estado     = c.Estado,
            Paciente   = DatosApp.Pacientes.FirstOrDefault(p => p.Id == c.PacienteId),
            Medico     = DatosApp.Medicos.FirstOrDefault(m => m.Id == c.MedicoId)
        }).ToList();

    public IActionResult Index()
        => View(ConNavegacion(DatosApp.Citas));

    public IActionResult PorPaciente(int pacienteId)
        => View(ConNavegacion(DatosApp.Citas.Where(c => c.PacienteId == pacienteId)));

    public IActionResult Crear()
    {
        CargarListas();
        return View(new Cita { Fecha = DateOnly.FromDateTime(DateTime.Today), Hora = new TimeOnly(9, 0) });
    }

    [HttpPost]
    public IActionResult Crear(Cita cita)
    {
        if (!ModelState.IsValid) { CargarListas(); return View(cita); }
        citaServicio.Crear(cita);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Editar(int id)
    {
        var cita = DatosApp.Citas.FirstOrDefault(c => c.Id == id);
        if (cita == null) return NotFound();
        CargarListas();
        return View(cita);
    }

    [HttpPost]
    public IActionResult Editar(Cita cita)
    {
        if (!ModelState.IsValid) { CargarListas(); return View(cita); }
        citaServicio.Actualizar(cita);
        if (cita.Estado == EstadoCita.Confirmada)
            citaService.Confirmar(cita);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Eliminar(int id)
    {
        citaServicio.Eliminar(id);
        return RedirectToAction(nameof(Index));
    }

    private void CargarListas()
    {
        ViewBag.Pacientes = DatosApp.Pacientes;
        ViewBag.Medicos   = DatosApp.Medicos;
        ViewBag.Estados   = Enum.GetValues<EstadoCita>();
    }
}
