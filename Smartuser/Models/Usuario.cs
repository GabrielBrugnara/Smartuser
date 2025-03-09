using System;

namespace Smartuser.Models
{
    public class Usuario
    {
        public int ID { get; set; }  // ID GERADO AUTOMATICAMENTE
        public string Nome { get; set; }  // NOME DO USUÁRIO
        public string Sobrenome { get; set; }  // SOBRENOME DO USUÁRIO
        public string Email { get; set; }  // EMAIL DO USUÁRIO
        public DateTime DataCriacao { get; set; }  // DATA DE CRIAÇÃO (REGISTRO DO USUÁRIO)
        public DateTime? UltimaAlteracao { get; set; }  // DATA DA ÚLTIMA ATUALIZAÇÃO, CASO HAJA
    }
}
