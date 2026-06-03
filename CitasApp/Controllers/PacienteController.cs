using CitasApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers;

public class PacienteController : Controller
{
    public IActionResult Index()
    {
        return View(DatosApp.Pacientes);
    }

    public IActionResult Detalle(int id)
    {
        var paciente = DatosApp.Pacientes.FirstOrDefault(p => p.Id == id);
        if (paciente == null) return NotFound();
        return View(paciente);
    }
}
