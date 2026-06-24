using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Infrastructure.Observers
{
    public class EmailObserver : ICitaObserver
    {
        public void Notificar(Cita cita, string evento) =>
            Console.WriteLine(
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [EMAIL] Simulando envío → " +
                $"Cita #{cita.Id} ha sido {evento} " +
                $"para el {cita.Fecha:dd/MM/yyyy} a las {cita.Hora:HH\\:mm}");
    }
}
