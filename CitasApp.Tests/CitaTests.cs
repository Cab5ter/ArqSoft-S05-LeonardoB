using CitasApp.Models;
using Xunit;

namespace CitasApp.Tests;

public class CitaTests
{
    [Fact]
    public void Cita_EstadoPorDefecto_EsPendiente()
    {
        // Arrange
        var cita = new Cita();

        // Act
        var estado = cita.Estado;

        // Assert
        Assert.Equal(EstadoCita.Pendiente, estado);
    }

    [Fact]
    public void Cita_AsignaFechaYHora_Correctamente()
    {
        // Arrange
        var cita = new Cita
        {
            Id = 1,
            PacienteId = 2,
            MedicoId = 3,
            Fecha = new DateOnly(2026, 6, 1),
            Hora = new TimeOnly(9, 30),
            Motivo = "Consulta general",
            Estado = EstadoCita.Confirmada
        };

        // Act & Assert
        Assert.Equal(new DateOnly(2026, 6, 1), cita.Fecha);
        Assert.Equal(new TimeOnly(9, 30), cita.Hora);
        Assert.Equal("Consulta general", cita.Motivo);
        Assert.Equal(EstadoCita.Confirmada, cita.Estado);
    }

    [Fact]
    public void Cita_RelacionesPorDefecto_SonNulas()
    {
        // Arrange
        var cita = new Cita();

        // Act & Assert
        Assert.Null(cita.Paciente);
        Assert.Null(cita.Medico);
        Assert.Equal("", cita.Motivo);
    }
}
