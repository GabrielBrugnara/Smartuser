using System.ComponentModel.DataAnnotations;

namespace Smartuser.Models
{
    public class FaturaProduto
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int FaturaID { get; set; }
        public Fatura Fatura { get; set; }  // Relacionamento com Fatura

        [Required]
        public int ProdutoID { get; set; }
        public Produto Produto { get; set; }  // Relacionamento com Produto

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public decimal Preco { get; set; }
    }
}
