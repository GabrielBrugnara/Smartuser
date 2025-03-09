using System.Collections.Generic;

namespace Smartuser.Models
{
    public class Grupo
    {
        public int ID { get; set; }
        public string Nome { get; set; }

        // Relacionamento reverso com Itens
        public ICollection<Item> Itens { get; set; } = new List<Item>();
    }
}
