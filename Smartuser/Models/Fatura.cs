using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smartuser.Models
{
    public class Fatura
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "O cliente é obrigatório.")]
        public int ClienteID { get; set; }
        [ForeignKey("ClienteID")]
        [Display(Name = "Cliente")]
        public Cliente? Cliente { get; set; } // <- opcional e sem [Required] PS:Isso resolveu minha vida haha//

        [Required]
        [Display(Name = "Data da Venda")]
        [DataType(DataType.Date)]
        public DateTime DataVenda { get; set; }

        [Display(Name = "Total Geral da Fatura")]
        public decimal TotalGeral { get; set; }

        [Display(Name = "Quantidade Total de Produtos")]
        public int TotalProdutos { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataUltimaAtualizacao { get; set; }

        public ICollection<FaturaProduto> FaturaProdutos { get; set; } = new List<FaturaProduto>();
    }
}
