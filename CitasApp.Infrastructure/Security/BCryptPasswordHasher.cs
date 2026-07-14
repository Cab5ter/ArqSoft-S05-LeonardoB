using CitasApp.Interfaces;

namespace CitasApp.Infrastructure.Security
{
    // Adaptador de salida para el hashing de contraseñas usando BCrypt.
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verificar(string password, string hash) =>
            BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
