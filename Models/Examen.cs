namespace ApiExamen.Models
{
    public class Examen
    {
        public int idExamen { get; set; }
        public string? titulo { get; set; }
        public string? descripcion { get; set; }
        public TimeSpan tiempoLimite { get; set; }  // Corregido a TimeSpan
    }
}