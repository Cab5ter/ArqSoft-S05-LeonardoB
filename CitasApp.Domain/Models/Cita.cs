namespace CitasApp.Models;

public class Cita
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly Hora { get; set; }
    public string Motivo { get; set; } = "";
    public EstadoCita Estado { get; set; }

    public Paciente? Paciente { get; set; }
    public Medico? Medico { get; set; }
}
