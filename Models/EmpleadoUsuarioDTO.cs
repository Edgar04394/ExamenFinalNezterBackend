namespace ApiExamen.Models
{
    public class EmpleadoUsuarioDTO
    {
        // Datos del empleado
        public string Nombre { get; set; } = null!;
        public string ApellidoPaterno { get; set; } = null!;
        public string? ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaInicioContrato { get; set; }
        public int IdPuesto { get; set; }

        // Datos para el login
        public string Usuario { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
    }
}
