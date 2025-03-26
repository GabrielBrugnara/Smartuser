using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using Smartuser.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Smartuser.Controllers
{
    public class FornecedorController : Controller
    {
        private readonly SmartuserContext _context;

        public FornecedorController(SmartuserContext context)
        {
            _context = context;
        }

        // GET: Fornecedor/ListaDeFornecedores
        public async Task<IActionResult> ListaDeFornecedores()
        {
            var fornecedores = await _context.Fornecedores
                                             .Include(f => f.GrupoFornecedor)
                                             .ToListAsync();

            ViewBag.Grupos = await _context.GrupoFornecedores.ToListAsync();
            return View(fornecedores);
        }

        // GET: Fornecedor/FiltrarTabela
        public async Task<IActionResult> FiltrarTabela(string busca, string empresa, int? grupoFornecedorId)
        {
            var query = _context.Fornecedores
                                .Include(f => f.GrupoFornecedor)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(busca))
            {
                query = query.Where(f =>
                    f.Empresa.Contains(busca) ||
                    f.GrupoFornecedor.Nome.Contains(busca) ||
                    f.CNPJ.Contains(busca));
            }

            if (!string.IsNullOrEmpty(empresa))
                query = query.Where(f => f.Empresa.Contains(empresa));

            if (grupoFornecedorId.HasValue)
                query = query.Where(f => f.GrupoFornecedorID == grupoFornecedorId);

            var filtrados = await query.ToListAsync();
            return PartialView("_TabelaFornecedores", filtrados);
        }

        // GET: Fornecedor/Criar
        public IActionResult Criar()
        {
            ViewBag.Grupos = _context.GrupoFornecedores.ToList();
            return View();
        }

        // POST: Fornecedor/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Fornecedor fornecedor)
        {
            ViewBag.Grupos = _context.GrupoFornecedores.ToList();

            if (!ModelState.IsValid)
                return View(fornecedor);

            fornecedor.DataCriacao = DateTime.Now;
            _context.Fornecedores.Add(fornecedor);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListaDeFornecedores));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao salvar: " + ex.Message);
                return View(fornecedor);
            }
        }

        // GET: Fornecedor/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null)
                return NotFound();

            ViewBag.Grupos = _context.GrupoFornecedores.ToList();
            return View(fornecedor);
        }

        // POST: Fornecedor/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Fornecedor fornecedor)
        {
            if (id != fornecedor.ID)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Grupos = _context.GrupoFornecedores.ToList();
                return View(fornecedor);
            }

            try
            {
                var noBanco = await _context.Fornecedores.FindAsync(id);
                if (noBanco == null)
                    return NotFound();

                noBanco.Empresa = fornecedor.Empresa;
                noBanco.CNPJ = fornecedor.CNPJ;
                noBanco.Telefone = fornecedor.Telefone;
                noBanco.Email = fornecedor.Email;
                noBanco.Endereco = fornecedor.Endereco;
                noBanco.GrupoFornecedorID = fornecedor.GrupoFornecedorID;
                noBanco.DataUltimaAtualizacao = DateTime.Now;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListaDeFornecedores));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Erro ao atualizar: " + ex.Message);
                ViewBag.Grupos = _context.GrupoFornecedores.ToList();
                return View(fornecedor);
            }
        }

        // GET: Fornecedor/Deletar/5
        public async Task<IActionResult> Deletar(int? id)
        {
            if (id == null)
                return NotFound();

            var fornecedor = await _context.Fornecedores
                                           .Include(f => f.GrupoFornecedor)
                                           .FirstOrDefaultAsync(f => f.ID == id);
            if (fornecedor == null)
                return NotFound();

            return View(fornecedor);
        }

        // POST: Fornecedor/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletarConfirmado(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor != null)
            {
                _context.Fornecedores.Remove(fornecedor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListaDeFornecedores));
        }

        // POST: GrupoFornecedor/CriarViaModal
        [HttpPost]
        public async Task<IActionResult> CriarViaModal([FromBody] GrupoFornecedor grupo)
        {
            if (string.IsNullOrWhiteSpace(grupo.Nome))
                return Json(new { success = false, message = "Nome do grupo obrigatório." });

            var novoGrupo = new GrupoFornecedor { Nome = grupo.Nome };
            _context.GrupoFornecedores.Add(novoGrupo);
            await _context.SaveChangesAsync();

            return Json(new { success = true, grupo = new { id = novoGrupo.ID, nome = novoGrupo.Nome } });
        }
    }
}
