namespace ApiExamen.Models
{
    public class Usuario
    {
        public int idUsuario { get; set; }
        public string? usuario { get; set; }
        public string? contrasena { get; set; }
        public string? rol { get; set; } // "Administrador" o "Empleado"
        public int? codigoEmpleado { get; set; }
    }
}
