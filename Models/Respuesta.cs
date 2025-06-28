namespace ApiExamen.Models
{
    public class Respuesta
    {
        public int idRespuesta { get; set; }
        public int idPregunta { get; set; }
        public string? textoRespuesta { get; set; }
        public int valor { get; set; }
        public int idClasificacion { get; set; }
    }

}