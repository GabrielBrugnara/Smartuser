using System.Collections.Generic;

namespace Smartuser.Models
{
    /// <summary>
    /// Representa um grupo de itens no sistema Smartuser.
    /// </summary>
    public class Grupo
    {
        /// <summary>
        /// Identificador único do grupo.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nome do grupo.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Coleção de itens que pertencem a este grupo.
        /// </summary>
        public ICollection<Item> Itens { get; set; } = new List<Item>();
    }
}
