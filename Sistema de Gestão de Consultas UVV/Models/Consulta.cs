using System.ComponentModel.DataAnnotations;

namespace SistemaConsultasUVV.Models
{
    public class Consulta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a especialidade.")]
        [StringLength(80)]
        public string Especialidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a data e hora.")]
        public DateTime DataHora { get; set; }

        [Required(ErrorMessage = "Informe a descrição.")]
        [StringLength(500)]
        public string Descricao { get; set; } = string.Empty;

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}