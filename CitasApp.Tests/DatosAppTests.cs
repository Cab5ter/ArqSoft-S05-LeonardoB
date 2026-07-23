using CitasApp.Data;
using CitasApp.Models;
using Xunit;

namespace CitasApp.Tests;

public class DatosAppTests
{
    [Fact]
    public void SiguienteIdPaciente_ListaVacia_RetornaUno()
    {
        // Arrange
        DatosApp.Pacientes = [];

        // Act
        var siguiente = DatosApp.SiguienteIdPaciente();

        // Assert
        Assert.Equal(1, siguiente);
    }

    [Fact]
    public void SiguienteIdMedico_ConElementos_RetornaMaximoMasUno()
    {
        // Arrange
        DatosApp.Medicos =
        [
            new() { Id = 3 },
            new() { Id = 8 },
            new() { Id = 5 }
        ];

        // Act
        var siguiente = DatosApp.SiguienteIdMedico();

        // Assert
        Assert.Equal(9, siguiente);
    }

    [Fact]
    public void SiguienteIdCita_ConUnElemento_RetornaSiguiente()
    {
        // Arrange
        DatosApp.Citas =
        [
            new() { Id = 1 }
        ];

        // Act
        var siguiente = DatosApp.SiguienteIdCita();

        // Assert
        Assert.Equal(2, siguiente);
    }
}
