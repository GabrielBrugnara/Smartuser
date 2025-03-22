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
                                         .Include(p => p.GrupoProduto)
                                         .ToListAsync();
            return View(produtos);
        }

        // GET: Produto/Criar
        public IActionResult Criar()
        {
            ViewBag.Grupos = _context.GrupoProdutos.ToList();
            return View();
        }

        // POST: Produto/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Produto produto)
        {
            ViewBag.Grupos = _context.GrupoProdutos.ToList();

            if (!ModelState.IsValid)
                return View(produto);

            var grupoIdStr = Request.Form["GrupoProdutoID"].ToString();
            if (string.IsNullOrEmpty(grupoIdStr) || !int.TryParse(grupoIdStr, out int grupoProdutoID))
            {
                ModelState.AddModelError("GrupoProdutoID", "Selecione um grupo válido.");
                return View(produto);
            }

            var grupo = await _context.GrupoProdutos.FindAsync(grupoProdutoID);
            if (grupo == null)
            {
                ModelState.AddModelError("GrupoProdutoID", "Grupo não encontrado.");
                return View(produto);
            }

            produto.GrupoProduto = grupo;
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
                                        .Include(p => p.GrupoProduto)
                                        .FirstOrDefaultAsync(p => p.ID == id);
            if (produto == null)
                return NotFound();

            ViewBag.Grupos = _context.GrupoProdutos.ToList();
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

                    var grupoIdStr = Request.Form["GrupoProdutoID"].ToString();
                    if (!string.IsNullOrEmpty(grupoIdStr) && int.TryParse(grupoIdStr, out int grupoProdutoID))
                    {
                        var grupo = await _context.GrupoProdutos.FindAsync(grupoProdutoID);
                        if (grupo != null)
                        {
                            produtoNoBanco.GrupoProduto = grupo;
                        }
                        else
                        {
                            ModelState.AddModelError("GrupoProdutoID", "Grupo não encontrado.");
                            ViewBag.Grupos = _context.GrupoProdutos.ToList();
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

            ViewBag.Grupos = _context.GrupoProdutos.ToList();
            return View(produto);
        }

        // GET: Produto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var produto = await _context.Produtos
                                        .Include(p => p.GrupoProduto)
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
        public async Task<IActionResult> CriarGrupo(GrupoProduto grupo)
        {
            if (ModelState.IsValid)
            {
                _context.GrupoProdutos.Add(grupo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Criar));
            }
            return PartialView("_CreateGrupoModal", grupo);
        }
    }
}
