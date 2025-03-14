using System.Collections.Generic;

namespace Smartuser.Models
{
    public class Grupo
    {
        public int GrupoID { get; set; }
        public string Nome { get; set; }

        // Nome corrigido para refletir a classe "Produto" no singular
        public ICollection<Produto> Produto { get; set; } = new List<Produto>();
    }
}
