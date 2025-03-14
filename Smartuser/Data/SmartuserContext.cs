using Microsoft.EntityFrameworkCore;
using Smartuser.Models;

namespace Smartuser.Data
{
    public class SmartuserContext : DbContext
    {
        public SmartuserContext(DbContextOptions<SmartuserContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Fatura> Faturas { get; set; }
        public DbSet<FaturaProduto> FaturaProdutos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacionamento entre Produto e Grupo
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Grupo) // Relacionamento com o Grupo
                .WithMany(g => g.Produto) // Um Grupo tem muitos Produtos
                .HasForeignKey(p => p.GrupoID) // Usamos a chave estrangeira "GrupoID"
                .OnDelete(DeleteBehavior.Cascade); // Exclusão em cascata

            // Relacionamento entre Fatura e Cliente
            modelBuilder.Entity<Fatura>()
                .ToTable("Faturas")
                .HasOne(f => f.Cliente)
                .WithMany()
                .HasForeignKey("ClienteID")
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento entre FaturaProduto e Fatura
            modelBuilder.Entity<FaturaProduto>()
                .ToTable("FaturaProdutos")
                .HasOne(fp => fp.Fatura)
                .WithMany(f => f.FaturaProdutos)
                .HasForeignKey(fp => fp.FaturaID)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento entre FaturaProduto e Produto
            modelBuilder.Entity<FaturaProduto>()
                .HasOne(fp => fp.Produto)
                .WithMany()
                .HasForeignKey(fp => fp.ProdutoID)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
