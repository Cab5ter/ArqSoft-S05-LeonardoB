using CitasApp.Application.Services;
using CitasApp.Models;
using CitasApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers;

public class CitaController(
    CitaServicio citaServicio,
    CitaService citaService,
    ICitaConsulta citaConsulta) : Controller
{
    public IActionResult Index()
        => View(citaConsulta.ObtenerTodas());

    public IActionResult PorPaciente(int pacienteId)
        => View(citaConsulta.ObtenerPorPaciente(pacienteId));

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
        var cita = citaConsulta.ObtenerPorId(id);
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
        ViewBag.Pacientes = citaConsulta.ObtenerPacientes();
        ViewBag.Medicos   = citaConsulta.ObtenerMedicos();
        ViewBag.Estados   = Enum.GetValues<EstadoCita>();
    }
}
