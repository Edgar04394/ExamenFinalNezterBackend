using ApiExamen.Models;

namespace ApiExamen.Interfaces
{
    public interface IAsignacionService
    {
        Task Asignar(Asignacion asignacion);
        Task EliminarAsignacion(int idAsignacion);
        Task<List<Asignacion>> ConsultarPorEmpleado(int codigoEmpleado);
    }
} 