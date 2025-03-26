using System.Collections.Generic;

namespace Smartuser.Models
{
    public class GrupoFornecedor
    {
        public int ID { get; set; }
        public string Nome { get; set; }

        public ICollection<Fornecedor> Fornecedores { get; set; } = new List<Fornecedor>();
    }
}
