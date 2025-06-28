namespace ApiExamen.Models
{
    public class Empleado
    {
        public int codigoEmpleado { get; set; }
        public string? nombre { get; set; }
        public string? apellidoPaterno { get; set; }
        public string? apellidoMaterno { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public DateTime fechaInicioContrato { get; set; }
        public int idPuesto { get; set; }
    }
}