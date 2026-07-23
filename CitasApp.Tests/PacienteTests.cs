using CitasApp.Models;
using Xunit;

namespace CitasApp.Tests;

public class PacienteTests
{
    [Fact]
    public void Paciente_PorDefecto_TieneCadenasVacias()
    {
        // Arrange
        var paciente = new Paciente();

        // Act & Assert
        Assert.Equal("", paciente.Nombre);
        Assert.Equal("", paciente.Apellido);
        Assert.Equal("", paciente.Email);
        Assert.Equal("", paciente.Telefono);
    }

    [Fact]
    public void Paciente_AsignaPropiedades_Correctamente()
    {
        // Arrange
        var paciente = new Paciente
        {
            Id = 1,
            Nombre = "Ana",
            Apellido = "García",
            Email = "ana@mail.com",
            Telefono = "555-0001"
        };

        // Act & Assert
        Assert.Equal(1, paciente.Id);
        Assert.Equal("Ana", paciente.Nombre);
        Assert.Equal("García", paciente.Apellido);
        Assert.Equal("ana@mail.com", paciente.Email);
        Assert.Equal("555-0001", paciente.Telefono);
    }

    [Fact]
    public void Paciente_IdPorDefecto_EsCero()
    {
        // Arrange
        var paciente = new Paciente();

        // Act
        var id = paciente.Id;

        // Assert
        Assert.Equal(0, id);
    }
}
