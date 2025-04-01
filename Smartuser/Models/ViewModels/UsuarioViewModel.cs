using Smartuser.Models;

namespace Smartuser.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public int Id { get; set; } // <- ESSENCIAL

        public string Nome { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }

        public List<int> PermissoesSelecionadas { get; set; } = new();
        public List<Permissao> TodasAsPermissoes { get; set; } = new();
    }
}
