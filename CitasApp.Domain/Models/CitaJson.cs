namespace CitasApp.Models;

public class CitaJson
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public string Fecha { get; set; } = "";
    public string Hora { get; set; } = "";
    public string Motivo { get; set; } = "";
    public EstadoCita Estado { get; set; }
}
