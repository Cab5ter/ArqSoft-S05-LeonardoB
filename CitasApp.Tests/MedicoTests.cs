using CitasApp.Models;
using Xunit;

namespace CitasApp.Tests;

public class MedicoTests
{
    [Fact]
    public void NombreCompleto_CombinaNombreYApellido()
    {
        // Arrange
        var medico = new Medico { Nombre = "Carlos", Apellido = "Reyes" };

        // Act
        var resultado = medico.NombreCompleto;

        // Assert
        Assert.Equal("Carlos Reyes", resultado);
    }

    [Fact]
    public void Medico_PorDefecto_TieneCadenasVacias()
    {
        // Arrange
        var medico = new Medico();

        // Act & Assert
        Assert.Equal("", medico.Nombre);
        Assert.Equal("", medico.Apellido);
        Assert.Equal("", medico.Especialidad);
        Assert.Equal("", medico.NumeroLicencia);
    }

    [Fact]
    public void Medico_AsignaPropiedades_Correctamente()
    {
        // Arrange
        var medico = new Medico
        {
            Id = 7,
            Nombre = "Patricia",
            Apellido = "Vega",
            Especialidad = "Pediatría",
            NumeroLicencia = "PD-20835"
        };

        // Act & Assert
        Assert.Equal(7, medico.Id);
        Assert.Equal("Pediatría", medico.Especialidad);
        Assert.Equal("PD-20835", medico.NumeroLicencia);
        Assert.Equal("Patricia Vega", medico.NombreCompleto);
    }
}
