using CitasApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers;

public class MedicoController : Controller
{
    public IActionResult Index()
    {
        return View(DatosApp.Medicos);
    }

    public IActionResult Detalle(int id)
    {
        var medico = DatosApp.Medicos.FirstOrDefault(m => m.Id == id);
        if (medico == null) return NotFound();
        return View(medico);
    }
}
