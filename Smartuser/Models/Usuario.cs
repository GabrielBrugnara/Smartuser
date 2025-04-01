namespace Smartuser.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataUltimaAtualizacao { get; set; }
        public ICollection<UsuarioPermissao> Permissoes { get; set; }
    }
}
