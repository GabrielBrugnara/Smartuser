using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Smartuser.Models
{
    public class Fatura
    {
        public int ID { get; set; }

        public Cliente Cliente { get; set; }  // Cliente vinculado à fatura

        [Required]
        [Display(Name = "Data da Venda")]
        [DataType(DataType.Date)]
        public DateTime DataVenda { get; set; }

        [Required]
        [Display(Name = "Total Geral da Fatura")]
        public decimal TotalGeral { get; set; } // Altere para TotalGeral para refletir o nome correto da coluna no banco de dados

        [Required]
        [Display(Name = "Quantidade Total de Produtos")]
        public int TotalProdutos { get; set; } // Soma das quantidades dos produtos na fatura

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataUltimaAtualizacao { get; set; }

        // Itens da fatura
        public ICollection<FaturaProduto> FaturaProdutos { get; set; } = new List<FaturaProduto>();
    }
}
