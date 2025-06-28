namespace ApiExamen.Models
{
    public class Asignacion
    {
        public int idAsignacion { get; set; }
        public int idExamen { get; set; }
        public int codigoEmpleado { get; set; }
        public DateTime fechaAsignacion { get; set; }
        public string? nombre_examen { get; set; }
        public string? descripcion { get; set; }
        public string? tiempoLimite { get; set; }
    }

}
