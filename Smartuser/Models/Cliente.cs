using System.ComponentModel.DataAnnotations;

namespace Smartuser.Models
{
    // Modelo para cadastro de clientes
    public class Cliente
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O sobrenome é obrigatório.")]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "O CPF/CNPJ é obrigatório.")]
        [Display(Name = "CPF/CNPJ")]
        public string CPFCNPJ { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        public string Telefone { get; set; }
    }
}
