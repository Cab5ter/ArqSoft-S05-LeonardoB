using CitasApp.Data;
using CitasApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers;

public class MedicoController : Controller
{
    public IActionResult Index()
        => View(DatosApp.Medicos);

    public IActionResult Detalle(int id)
    {
        var medico = DatosApp.Medicos.FirstOrDefault(m => m.Id == id);
        if (medico == null) return NotFound();
        return View(medico);
    }

    public IActionResult Crear() => View(new Medico());

    [HttpPost]
    public IActionResult Crear(Medico medico)
    {
        if (!ModelState.IsValid) return View(medico);
        medico.Id = DatosApp.SiguienteIdMedico();
        DatosApp.Medicos.Add(medico);
        DatosApp.GuardarMedicos();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Editar(int id)
    {
        var medico = DatosApp.Medicos.FirstOrDefault(m => m.Id == id);
        if (medico == null) return NotFound();
        return View(medico);
    }

    [HttpPost]
    public IActionResult Editar(Medico medico)
    {
        if (!ModelState.IsValid) return View(medico);
        var existente = DatosApp.Medicos.FirstOrDefault(m => m.Id == medico.Id);
        if (existente == null) return NotFound();
        existente.Nombre = medico.Nombre;
        existente.Apellido = medico.Apellido;
        existente.Especialidad = medico.Especialidad;
        existente.NumeroLicencia = medico.NumeroLicencia;
        DatosApp.GuardarMedicos();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Eliminar(int id)
    {
        var medico = DatosApp.Medicos.FirstOrDefault(m => m.Id == id);
        if (medico != null)
        {
            DatosApp.Medicos.Remove(medico);
            DatosApp.GuardarMedicos();
        }
        return RedirectToAction(nameof(Index));
    }
}
