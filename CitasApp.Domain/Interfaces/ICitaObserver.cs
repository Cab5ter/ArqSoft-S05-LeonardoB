using CitasApp.Models;

namespace CitasApp.Interfaces
{
    public interface ICitaObserver
    {
        void Notificar(Cita cita, string evento);
    }
}
