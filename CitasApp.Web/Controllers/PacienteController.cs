using CitasApp.Data;
using CitasApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers;

public class PacienteController : Controller
{
    public IActionResult Index()
        => View(DatosApp.Pacientes);

    public IActionResult Detalle(int id)
    {
        var paciente = DatosApp.Pacientes.FirstOrDefault(p => p.Id == id);
        if (paciente == null) return NotFound();
        return View(paciente);
    }

    public IActionResult Crear() => View(new Paciente());

    [HttpPost]
    public IActionResult Crear(Paciente paciente)
    {
        if (!ModelState.IsValid) return View(paciente);
        paciente.Id = DatosApp.SiguienteIdPaciente();
        DatosApp.Pacientes.Add(paciente);
        DatosApp.GuardarPacientes();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Editar(int id)
    {
        var paciente = DatosApp.Pacientes.FirstOrDefault(p => p.Id == id);
        if (paciente == null) return NotFound();
        return View(paciente);
    }

    [HttpPost]
    public IActionResult Editar(Paciente paciente)
    {
        if (!ModelState.IsValid) return View(paciente);
        var existente = DatosApp.Pacientes.FirstOrDefault(p => p.Id == paciente.Id);
        if (existente == null) return NotFound();
        existente.Nombre = paciente.Nombre;
        existente.Apellido = paciente.Apellido;
        existente.Email = paciente.Email;
        existente.Telefono = paciente.Telefono;
        DatosApp.GuardarPacientes();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Eliminar(int id)
    {
        var paciente = DatosApp.Pacientes.FirstOrDefault(p => p.Id == id);
        if (paciente != null)
        {
            DatosApp.Pacientes.Remove(paciente);
            DatosApp.GuardarPacientes();
        }
        return RedirectToAction(nameof(Index));
    }
}
