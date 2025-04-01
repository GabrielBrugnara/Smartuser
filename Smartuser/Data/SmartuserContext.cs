using Microsoft.EntityFrameworkCore;
using Smartuser.Models;

namespace Smartuser.Data
{
    public class SmartuserContext : DbContext
    {
        public SmartuserContext(DbContextOptions<SmartuserContext> options) : base(options) { }

        // Tabelas do sistema
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<GrupoProduto> GrupoProdutos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Fatura> Faturas { get; set; }
        public DbSet<FaturaProduto> FaturaProdutos { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<GrupoFornecedor> GrupoFornecedores { get; set; }

        // Tabelas de autenticação
        public DbSet<Permissao> Permissoes { get; set; }
        public DbSet<UsuarioPermissao> UsuarioPermissoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Produto ↔ GrupoProduto
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.GrupoProduto)
                .WithMany(gp => gp.Produtos)
                .HasForeignKey(p => p.GrupoProdutoID)
                .OnDelete(DeleteBehavior.Cascade);

            // Fatura ↔ Cliente
            modelBuilder.Entity<Fatura>()
                .ToTable("Faturas")
                .HasOne(f => f.Cliente)
                .WithMany()
                .HasForeignKey("ClienteID")
                .OnDelete(DeleteBehavior.Restrict);

            // FaturaProduto ↔ Fatura
            modelBuilder.Entity<FaturaProduto>()
                .ToTable("FaturaProdutos")
                .HasOne(fp => fp.Fatura)
                .WithMany(f => f.FaturaProdutos)
                .HasForeignKey(fp => fp.FaturaID)
                .OnDelete(DeleteBehavior.Cascade);

            // FaturaProduto ↔ Produto
            modelBuilder.Entity<FaturaProduto>()
                .HasOne(fp => fp.Produto)
                .WithMany()
                .HasForeignKey(fp => fp.ProdutoID)
                .OnDelete(DeleteBehavior.Restrict);

            // MovimentacaoEstoque ↔ Produto
            modelBuilder.Entity<MovimentacaoEstoque>()
                .ToTable("MovimentacoesEstoque")
                .HasOne(m => m.Produto)
                .WithMany()
                .HasForeignKey(m => m.ProdutoID)
                .OnDelete(DeleteBehavior.Restrict);

            // Fornecedor ↔ GrupoFornecedor
            modelBuilder.Entity<Fornecedor>()
                .HasOne(f => f.GrupoFornecedor)
                .WithMany(g => g.Fornecedores)
                .HasForeignKey(f => f.GrupoFornecedorID)
                .OnDelete(DeleteBehavior.Cascade);

            // Usuario ↔ Permissao (relacionamento N:N)
            modelBuilder.Entity<UsuarioPermissao>()
                .HasKey(up => new { up.UsuarioId, up.PermissaoId });

            modelBuilder.Entity<UsuarioPermissao>()
                .HasOne(up => up.Usuario)
                .WithMany(u => u.Permissoes)
                .HasForeignKey(up => up.UsuarioId);

            modelBuilder.Entity<UsuarioPermissao>()
                .HasOne(up => up.Permissao)
                .WithMany()
                .HasForeignKey(up => up.PermissaoId);

            // 🔐 Seed: Usuário admin com todas as permissões fixas
            var adminId = 1;
            var senhaHashAdmin = BCrypt.Net.BCrypt.HashPassword("admin123");

            modelBuilder.Entity<Usuario>().HasData(new Usuario
            {
                Id = adminId,
                Nome = "Administrador do Sistema",
                Username = "admin",
                Email = "admin@smartser.com",
                SenhaHash = senhaHashAdmin,
                DataCriacao = DateTime.Now
            });

            var permissoesIniciais = new[]
            {
                new Permissao { Id = 1, Nome = "VisualizarProdutos" },
                new Permissao { Id = 2, Nome = "EditarProdutos" },
                new Permissao { Id = 3, Nome = "VisualizarFaturas" },
                new Permissao { Id = 4, Nome = "EditarFaturas" },
                new Permissao { Id = 5, Nome = "AcessarEstoque" },
                new Permissao { Id = 6, Nome = "GerenciarUsuarios" }
            };
            modelBuilder.Entity<Permissao>().HasData(permissoesIniciais);

            var usuarioPermissoes = permissoesIniciais.Select(p => new UsuarioPermissao
            {
                UsuarioId = adminId,
                PermissaoId = p.Id
            }).ToArray();
            modelBuilder.Entity<UsuarioPermissao>().HasData(usuarioPermissoes);

            base.OnModelCreating(modelBuilder);
        }
    }
}
