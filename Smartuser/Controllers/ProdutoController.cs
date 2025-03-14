using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using Smartuser.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Smartuser.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly SmartuserContext _context;

        public ProdutoController(SmartuserContext context)
        {
            _context = context;
        }

        // GET: Produto/ListaDeProdutos
        public async Task<IActionResult> ListaDeProdutos()
        {
            var produtos = await _context.Produtos
                                         .Include(p => p.Grupo)
                                         .ToListAsync();
            return View(produtos);
        }

        // GET: Produto/Criar
        public IActionResult Criar()
        {
            ViewBag.Grupos = _context.Grupos.ToList();
            return View();
        }

        // POST: Produto/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Produto produto)
        {
            ViewBag.Grupos = _context.Grupos.ToList();

            if (!ModelState.IsValid)
                return View(produto);

            var grupoIdStr = Request.Form["GrupoID"].ToString();
            if (string.IsNullOrEmpty(grupoIdStr) || !int.TryParse(grupoIdStr, out int grupoID))
            {
                ModelState.AddModelError("GrupoID", "Selecione um grupo válido.");
                return View(produto);
            }

            var grupo = await _context.Grupos.FindAsync(grupoID);
            if (grupo == null)
            {
                ModelState.AddModelError("GrupoID", "Grupo não encontrado.");
                return View(produto);
            }

            produto.Grupo = grupo;
            produto.DataCriacao = DateTime.Now;
            _context.Produtos.Add(produto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao salvar o produto: " + ex.Message);
                return View(produto);
            }

            return RedirectToAction(nameof(ListaDeProdutos));
        }

        // GET: Produto/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var produto = await _context.Produtos
                                        .Include(p => p.Grupo)
                                        .FirstOrDefaultAsync(p => p.ID == id);
            if (produto == null)
                return NotFound();

            ViewBag.Grupos = _context.Grupos.ToList();
            return View(produto);
        }

        // POST: Produto/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Produto produto)
        {
            if (id != produto.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var produtoNoBanco = await _context.Produtos.FindAsync(id);
                    if (produtoNoBanco == null)
                        return NotFound();

                    produtoNoBanco.Descricao = produto.Descricao;

                    var grupoIdStr = Request.Form["GrupoID"].ToString();
                    if (!string.IsNullOrEmpty(grupoIdStr) && int.TryParse(grupoIdStr, out int grupoID))
                    {
                        var grupo = await _context.Grupos.FindAsync(grupoID);
                        if (grupo != null)
                        {
                            produtoNoBanco.Grupo = grupo;
                        }
                        else
                        {
                            ModelState.AddModelError("GrupoID", "Grupo não encontrado.");
                            ViewBag.Grupos = _context.Grupos.ToList();
                            return View(produto);
                        }
                    }

                    produtoNoBanco.QuantidadeEstoque = produto.QuantidadeEstoque;
                    produtoNoBanco.Preco = produto.Preco;
                    produtoNoBanco.DataUltimaAtualizacao = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Produtos.Any(e => e.ID == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(ListaDeProdutos));
            }
            ViewBag.Grupos = _context.Grupos.ToList();
            return View(produto);
        }

        // GET: Produto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var produto = await _context.Produtos
                                        .Include(p => p.Grupo)
                                        .FirstOrDefaultAsync(m => m.ID == id);
            if (produto == null)
                return NotFound();

            return View(produto);
        }

        // POST: Produto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListaDeProdutos));
        }

        // POST: Produto/CriarGrupo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarGrupo(Grupo grupo)
        {
            if (ModelState.IsValid)
            {
                _context.Grupos.Add(grupo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Criar));
            }
            return PartialView("_CreateGrupoModal", grupo);
        }
    }
}
