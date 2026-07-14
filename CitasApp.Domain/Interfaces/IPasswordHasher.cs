namespace CitasApp.Interfaces
{
    // Puerto de salida para el hashing de contraseñas.
    // Interfaz pequeña y enfocada (ISP): separada del repositorio.
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verificar(string password, string hash);
    }
}
