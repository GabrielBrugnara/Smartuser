using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using Smartuser.Models;

namespace Smartuser.Controllers
{
    public class FaturaController : Controller
    {
        private readonly SmartuserContext _context;

        public FaturaController(SmartuserContext context)
        {
            _context = context;
        }

        // GET: Fatura/ListaDeFaturas
        public async Task<IActionResult> ListaDeFaturas()
        {
            var faturas = await _context.Faturas
                .Include(f => f.Cliente)
                .ToListAsync();
            return View(faturas);
        }

        // GET: Fatura/Criar
        public IActionResult Criar()
        {
            ViewBag.Clientes = _context.Clientes.ToList();  // Clientes são carregados normalmente
            ViewBag.Produtos = _context.Produtos
                .Include(p => p.Grupo) // Carrega o grupo associado ao produto, sem expor diretamente o GrupoID
                .ToList();
            return View();
        }

        // POST: Fatura/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Fatura fatura, int clienteId, int[] produtoIds, int[] quantidades)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = _context.Clientes.ToList();
                ViewBag.Produtos = _context.Produtos.ToList();
                return View(fatura);
            }

            // Define o ClienteID via propriedade sombra
            _context.Entry(fatura).Property("ClienteID").CurrentValue = clienteId;

            fatura.FaturaProdutos ??= new List<FaturaProduto>();

            decimal total = 0;
            int totalProdutos = 0;
            bool hasStockError = false;

            if (produtoIds != null && quantidades != null && produtoIds.Length == quantidades.Length)
            {
                for (int i = 0; i < produtoIds.Length; i++)
                {
                    var produto = await _context.Produtos.FindAsync(produtoIds[i]);
                    if (produto != null)
                    {
                        if (quantidades[i] > produto.QuantidadeEstoque)
                        {
                            ModelState.AddModelError("", $"Estoque insuficiente para '{produto.Descricao}'. Disponível: {produto.QuantidadeEstoque}.");
                            hasStockError = true;
                        }
                        else
                        {
                            total += produto.Preco * quantidades[i];
                            totalProdutos += quantidades[i];

                            fatura.FaturaProdutos.Add(new FaturaProduto
                            {
                                ProdutoID = produto.ID,
                                Quantidade = quantidades[i],
                                Preco = produto.Preco
                            });

                            // Reduz estoque
                            produto.QuantidadeEstoque -= quantidades[i];
                        }
                    }
                }
            }

            if (hasStockError)
            {
                ViewBag.Clientes = _context.Clientes.ToList();
                ViewBag.Produtos = _context.Produtos.ToList();
                return View(fatura);
            }

            fatura.TotalGeral = total;
            fatura.TotalProdutos = totalProdutos;
            fatura.DataCriacao = DateTime.Now;

            _context.Add(fatura);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListaDeFaturas));
        }

        // GET: Fatura/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var fatura = await _context.Faturas
                .Include(f => f.FaturaProdutos)
                .ThenInclude(fp => fp.Produto)
                .FirstOrDefaultAsync(f => f.ID == id);
            if (fatura == null)
                return NotFound();

            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Produtos = _context.Produtos.ToList();
            return View(fatura);
        }

        // POST: Fatura/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Fatura fatura, int clienteId, int[] produtoIds, int[] quantidades)
        {
            if (id != fatura.ID)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = _context.Clientes.ToList();
                ViewBag.Produtos = _context.Produtos.ToList();
                return View(fatura);
            }

            var faturaDb = await _context.Faturas
                .Include(f => f.FaturaProdutos)
                .FirstOrDefaultAsync(f => f.ID == id);

            if (faturaDb == null)
                return NotFound();

            // Restaura o estoque dos produtos removidos
            foreach (var faturaProduto in faturaDb.FaturaProdutos)
            {
                var produtoAntigo = await _context.Produtos.FindAsync(faturaProduto.ProdutoID);
                if (produtoAntigo != null)
                {
                    produtoAntigo.QuantidadeEstoque += faturaProduto.Quantidade;
                }
            }

            // Define o ClienteID via propriedade sombra
            _context.Entry(faturaDb).Property("ClienteID").CurrentValue = clienteId;

            _context.FaturaProdutos.RemoveRange(faturaDb.FaturaProdutos);
            faturaDb.FaturaProdutos.Clear();

            decimal total = 0;
            int totalProdutos = 0;
            bool hasStockError = false;

            if (produtoIds != null && quantidades != null && produtoIds.Length == quantidades.Length)
            {
                for (int i = 0; i < produtoIds.Length; i++)
                {
                    var produto = await _context.Produtos.FindAsync(produtoIds[i]);
                    if (produto != null)
                    {
                        if (quantidades[i] > produto.QuantidadeEstoque)
                        {
                            ModelState.AddModelError("", $"Estoque insuficiente para '{produto.Descricao}'. Disponível: {produto.QuantidadeEstoque}.");
                            hasStockError = true;
                        }
                        else
                        {
                            total += produto.Preco * quantidades[i];
                            totalProdutos += quantidades[i];

                            faturaDb.FaturaProdutos.Add(new FaturaProduto
                            {
                                ProdutoID = produto.ID,
                                Quantidade = quantidades[i],
                                Preco = produto.Preco,
                                FaturaID = faturaDb.ID
                            });

                            // Atualiza estoque
                            produto.QuantidadeEstoque -= quantidades[i];
                        }
                    }
                }
            }

            if (hasStockError)
            {
                ViewBag.Clientes = _context.Clientes.ToList();
                ViewBag.Produtos = _context.Produtos.ToList();
                return View(faturaDb);
            }

            faturaDb.TotalGeral = total;
            faturaDb.TotalProdutos = totalProdutos;
            faturaDb.DataUltimaAtualizacao = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Faturas.Any(e => e.ID == id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(ListaDeFaturas));
        }

        // GET: Fatura/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var fatura = await _context.Faturas
                .Include(f => f.Cliente)
                .FirstOrDefaultAsync(f => f.ID == id);
            if (fatura == null)
                return NotFound();

            return View(fatura);
        }

        // POST: Fatura/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fatura = await _context.Faturas.FindAsync(id);
            if (fatura != null)
            {
                _context.Faturas.Remove(fatura);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListaDeFaturas));
        }
    }
}
