using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Smartuser.Models
{
    public class Fornecedor
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public string Empresa { get; set; }

        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }

        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [Required]
        [Display(Name = "Grupo")]
        public int GrupoFornecedorID { get; set; }

        [ValidateNever]
        public GrupoFornecedor GrupoFornecedor { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataUltimaAtualizacao { get; set; }
    }
}
