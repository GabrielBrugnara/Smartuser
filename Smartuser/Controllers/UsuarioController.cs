using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Models;
using Smartuser.Models.ViewModels;
using Smartuser.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Smartuser.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly SmartuserContext _context;

        public UsuarioController(SmartuserContext context)
        {
            _context = context;
        }

        // LISTAR USUÁRIOS
        public IActionResult Index()
        {
            var usuarios = _context.Usuarios.ToList();
            return View("ListaUsuarios", usuarios);
        }

        // GET: Criar
        public IActionResult Criar()
        {
            var viewModel = new UsuarioViewModel
            {
                TodasAsPermissoes = _context.Permissoes.ToList()
            };

            return View(viewModel);
        }

        // POST: Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(UsuarioViewModel viewModel)
        {
            ModelState.Remove("TodasAsPermissoes");

            if (ModelState.IsValid)
            {
                var usuario = new Usuario
                {
                    Nome = viewModel.Nome,
                    Username = viewModel.Username,
                    Email = viewModel.Email,
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword(viewModel.Senha),
                    DataCriacao = DateTime.Now
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                foreach (var pId in viewModel.PermissoesSelecionadas)
                {
                    _context.UsuarioPermissoes.Add(new UsuarioPermissao
                    {
                        UsuarioId = usuario.Id,
                        PermissaoId = pId
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            viewModel.TodasAsPermissoes = _context.Permissoes.ToList();
            return View(viewModel);
        }

        // GET: Editar
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Permissoes)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            var viewModel = new UsuarioViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Username = usuario.Username,
                Email = usuario.Email,
                PermissoesSelecionadas = usuario.Permissoes.Select(p => p.PermissaoId).ToList(),
                TodasAsPermissoes = _context.Permissoes.ToList()
            };

            return View(viewModel);
        }

        // POST: Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(UsuarioViewModel viewModel, string novaSenha)
        {
            // Remover validações que não se aplicam na edição:
            ModelState.Remove("TodasAsPermissoes");
            // Remove todas as entradas relacionadas à "Senha" (garante remover "Senha" e variações)
            foreach (var key in ModelState.Keys.Where(k => k.Contains("Senha")).ToList())
            {
                ModelState.Remove(key);
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Permissoes)
                .FirstOrDefaultAsync(u => u.Id == viewModel.Id);

            if (usuario == null)
                return NotFound();

            // Atualiza os dados (exceto Senha, que só é atualizada se novaSenha for preenchida)
            if (usuario.Id != 1)
            {
                usuario.Nome = viewModel.Nome;
                usuario.Username = viewModel.Username;

                usuario.Permissoes.Clear();
                if (viewModel.PermissoesSelecionadas != null)
                {
                    foreach (var pId in viewModel.PermissoesSelecionadas)
                    {
                        usuario.Permissoes.Add(new UsuarioPermissao
                        {
                            PermissaoId = pId,
                            UsuarioId = usuario.Id
                        });
                    }
                }
            }

            usuario.Email = viewModel.Email;
            usuario.DataUltimaAtualizacao = DateTime.Now;

            // Se o campo de nova senha for preenchido, atualiza a senha; se não, mantém a senha atual.
            if (!string.IsNullOrWhiteSpace(novaSenha))
            {
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Deletar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deletar(int id)
        {
            if (id == 1)
                return Forbid(); // 🔒 Protege admin padrão

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
