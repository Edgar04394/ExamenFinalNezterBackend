using ApiExamen.Models;

namespace ApiExamen.Interfaces
{
    public interface IResultadoService
    {
        Task GuardarRespuestaEmpleado(RespuestaEmpleado respuesta);
        Task<List<object>> ObtenerResultadosHistoricos(int codigoEmpleado);
        Task GuardarEnHistorial(int idAsignacion);
        Task<List<ReportePromedioCompetencia>> ReportePromedioPorCompetencia(int idAsignacion);
        Task<List<object>> ObtenerResultadosPorExamen(int idExamen, int codigoEmpleado);
    }
} 