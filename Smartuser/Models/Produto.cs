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

        // Propriedade sombra para chave estrangeira
        public int GrupoID { get; set; }

        // O campo Grupo não está exposto diretamente, mas o relacionamento é mapeado
        [ValidateNever]
        public Grupo Grupo { get; set; }
    }
}
