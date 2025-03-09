using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Smartuser.Models
{
    public class Item
    {
        public int ID { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        public int QuantidadeEstoque { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }

        // O campo GrupoID não está exposto no modelo, pois usamos propriedade sombra.
        // A propriedade de navegação abaixo será ignorada na validação:
        [ValidateNever]
        public Grupo Grupo { get; set; }
    }
}
