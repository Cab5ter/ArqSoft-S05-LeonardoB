using CitasApp.Data;
using CitasApp.Models;

namespace CitasApp.Services
{
    public class CitaServicio
    {
        public void Crear(Cita cita)
        {
            cita.Id = DatosApp.SiguienteIdCita();
            DatosApp.Citas.Add(cita);
            DatosApp.GuardarCitas();
        }

        public void Actualizar(Cita cita)
        {
            var existente = DatosApp.Citas.FirstOrDefault(c => c.Id == cita.Id);
            if (existente == null) return;
            existente.PacienteId = cita.PacienteId;
            existente.MedicoId   = cita.MedicoId;
            existente.Fecha      = cita.Fecha;
            existente.Hora       = cita.Hora;
            existente.Motivo     = cita.Motivo;
            existente.Estado     = cita.Estado;
            DatosApp.GuardarCitas();
        }

        public void Eliminar(int id)
        {
            var cita = DatosApp.Citas.FirstOrDefault(c => c.Id == id);
            if (cita == null) return;
            DatosApp.Citas.Remove(cita);
            DatosApp.GuardarCitas();
        }
    }
}
