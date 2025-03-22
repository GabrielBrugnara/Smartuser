using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Smartuser.Models
{
    public class Produto
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Descrição do Produto")]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Quantidade em Estoque")]
        public int QuantidadeEstoque { get; set; }

        [Required]
        [Display(Name = "Preço Unitário")]
        public decimal Preco { get; set; }

        [Required]
        [Display(Name = "Data de Criação")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [Display(Name = "Última Atualização")]
        public DateTime? DataUltimaAtualizacao { get; set; }

        // Corrigido: agora usa GrupoProdutoID
        public int GrupoProdutoID { get; set; }

        [ValidateNever]
        public GrupoProduto GrupoProduto { get; set; }
    }
}
