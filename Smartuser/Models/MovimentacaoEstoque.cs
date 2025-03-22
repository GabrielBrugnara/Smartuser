using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smartuser.Models
{
    public class MovimentacaoEstoque
    {
        public int ID { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        public string Tipo { get; set; } // Entrada, Saída, Ajuste Manual

        [Required]
        public int ProdutoID { get; set; }

        public Produto Produto { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public string Origem { get; set; } // Ex: Fatura #12, Manual
    }
}
