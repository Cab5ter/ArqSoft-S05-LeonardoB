using CitasApp.Models;

namespace CitasApp.Interfaces
{
    // Puerto de salida: la capa de aplicación depende de esta abstracción,
    // no de la persistencia concreta (DIP).
    public interface IUsuarioRepository
    {
        Usuario? ObtenerPorEmail(string email);
        void Agregar(Usuario usuario);
    }
}
