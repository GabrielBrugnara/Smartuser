using Microsoft.EntityFrameworkCore;
using Smartuser.Models;

namespace Smartuser.Data
{
    public class SmartuserContext : DbContext
    {
        public SmartuserContext(DbContextOptions<SmartuserContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<GrupoProduto> GrupoProdutos { get; set; } 
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Fatura> Faturas { get; set; }
        public DbSet<FaturaProduto> FaturaProdutos { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacionamento entre Produto e GrupoProduto
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.GrupoProduto)
                .WithMany(gp => gp.Produtos)
                .HasForeignKey(p => p.GrupoProdutoID)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Relacionamento entre MovimentacaoEstoque e Produto
            modelBuilder.Entity<MovimentacaoEstoque>()
                .ToTable("MovimentacoesEstoque")
                .HasOne(m => m.Produto)
                .WithMany()
                .HasForeignKey(m => m.ProdutoID)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
