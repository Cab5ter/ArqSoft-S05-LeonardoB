using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Infrastructure.Observers
{
    public class SmsObserver : ICitaObserver
    {
        public void Notificar(Cita cita, string evento) =>
            Console.WriteLine(
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [SMS]   Cita #{cita.Id} {evento} | " +
                $"Paciente={cita.PacienteId}  Médico={cita.MedicoId}  " +
                $"Fecha={cita.Fecha:dd/MM/yyyy}  Estado={cita.Estado}");
    }
}
