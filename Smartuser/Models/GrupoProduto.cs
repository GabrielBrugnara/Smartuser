using System.Collections.Generic;

namespace Smartuser.Models
{
    public class GrupoProduto
    {
        public int ID { get; set; }
        public string Nome { get; set; }

        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
