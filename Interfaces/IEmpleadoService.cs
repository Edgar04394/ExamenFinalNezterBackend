using ApiExamen.Models;

namespace ApiExamen.Interfaces
{
    public interface IEmpleadoService
    {
        Task<List<Empleado>> ObtenerTodos();
        Task<Empleado?> ObtenerPorCodigo(int codigoEmpleado);
        Task<EmpleadoUsuarioDTO?> ObtenerEmpleadoConCredenciales(string usuario, string contrasena);
        Task Crear(Empleado empleado);
        Task Actualizar(Empleado empleado);
        Task Eliminar(int codigoEmpleado);
        Task CrearEmpleadoConUsuario(EmpleadoUsuarioDTO dto);
        Task<IEnumerable<Empleado>> Consultar();
    }
} 