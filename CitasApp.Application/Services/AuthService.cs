using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Application.Services;

// Caso de uso de autenticación. Depende solo de abstracciones del dominio (DIP)
// y tiene una única responsabilidad: autenticar y registrar usuarios (SRP).
public class AuthService
{
    private readonly IUsuarioRepository _repo;
    private readonly IPasswordHasher _hasher;

    public AuthService(IUsuarioRepository repo, IPasswordHasher hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }

    // Devuelve el usuario si las credenciales son válidas; null en caso contrario.
    public Usuario? Autenticar(string email, string password)
    {
        var usuario = _repo.ObtenerPorEmail(email);
        if (usuario is null) return null;

        return _hasher.Verificar(password, usuario.PasswordHash) ? usuario : null;
    }

    public Usuario Registrar(string nombre, string email, string password, string rol = "Usuario")
    {
        var usuario = new Usuario
        {
            Nombre = nombre,
            Email = email,
            PasswordHash = _hasher.Hash(password),
            Rol = rol
        };
        _repo.Agregar(usuario);
        return usuario;
    }
}
