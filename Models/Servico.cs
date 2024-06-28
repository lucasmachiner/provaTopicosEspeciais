using System.ComponentModel.DataAnnotations;

namespace provasemestral.Models
{
    public class Servico
    {
        [Key]
        public string? Id { get; set; }
        public string? Nome {  get; set; }
        public decimal Preco { get; set; }
        public bool Status { get; set; }
    }
}
