using Microsoft.EntityFrameworkCore;
using Smartuser.Models;

namespace Smartuser.Data
{
    public class SmartuserContext : DbContext
    {
        public SmartuserContext(DbContextOptions<SmartuserContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Item> Itens { get; set; }
        public DbSet<Grupo> Grupos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Grupo)
                .WithMany(g => g.Itens)
                .HasForeignKey("GrupoID") // Propriedade sombra para o FK
                .HasPrincipalKey(g => g.ID);
        }
    }
}
