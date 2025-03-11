using Microsoft.EntityFrameworkCore;
using Smartuser.Models;

namespace Smartuser.Data
{
    public class SmartuserContext : DbContext
    {
        public SmartuserContext(DbContextOptions<SmartuserContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Item> Itens { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Cliente> Clientes { get; set; } // Novo cadastro de clientes

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Grupo)
                .WithMany(g => g.Itens)
                .HasForeignKey("GrupoID")
                .HasPrincipalKey(g => g.ID)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
