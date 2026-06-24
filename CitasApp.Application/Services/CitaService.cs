using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Application.Services;

public class CitaService
{
    private readonly List<ICitaObserver> _observadores = [];

    public void Suscribir(ICitaObserver observador) => _observadores.Add(observador);

    public void Confirmar(Cita cita)
    {
        foreach (var obs in _observadores)
            obs.Notificar(cita, "confirmada");
    }
}
