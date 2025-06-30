using ApiExamen.Models;
using System.Threading.Tasks;

namespace ApiExamen.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerUsuarioPorCredenciales(string usuario, string contrasena);
    }
} 