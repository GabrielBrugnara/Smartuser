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

        public async Task<IActionResult> ListaDeFaturas()
        {
            var faturas = await _context.Faturas
                .Include(f => f.Cliente)
                .ToListAsync();
            return View(faturas);
        }

        public IActionResult Criar()
        {
            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Produtos = _context.Produtos
                .Select(p => new
                {
                    p.ID,
                    p.Descricao,
                    p.Preco,
                    Estoque = p.QuantidadeEstoque,
                    GrupoNome = p.GrupoProduto.Nome
                })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Fatura fatura, int clienteId, int[] produtoIds, int[] quantidades)
        {
            if (Request.Form.TryGetValue("DataVenda", out var dataVendaString) &&
                DateTime.TryParse(dataVendaString, out var dataVenda))
            {
                fatura.DataVenda = dataVenda;
            }
            else
            {
                ModelState.AddModelError("", "Data da venda inválida.");
            }

            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null)
            {
                ModelState.AddModelError("Cliente", "Cliente não encontrado.");
            }
            else
            {
                fatura.Cliente = cliente;
                fatura.ClienteID = clienteId;
            }

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

                            produto.QuantidadeEstoque -= quantidades[i];
                        }
                    }
                }
            }

            if (!ModelState.IsValid || hasStockError)
            {
                ViewBag.Clientes = _context.Clientes.ToList();
                ViewBag.Produtos = _context.Produtos
                    .Select(p => new
                    {
                        p.ID,
                        p.Descricao,
                        p.Preco,
                        Estoque = p.QuantidadeEstoque,
                        GrupoNome = p.GrupoProduto.Nome
                    })
                    .ToList();
                return View(fatura);
            }

            fatura.TotalGeral = total;
            fatura.TotalProdutos = totalProdutos;
            fatura.DataCriacao = DateTime.Now;

            _context.Add(fatura);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListaDeFaturas));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();

            var fatura = await _context.Faturas
                .Include(f => f.FaturaProdutos)
                .ThenInclude(fp => fp.Produto)
                .FirstOrDefaultAsync(f => f.ID == id);

            if (fatura == null) return NotFound();

            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Produtos = _context.Produtos
                .Select(p => new
                {
                    p.ID,
                    p.Descricao,
                    p.Preco,
                    Estoque = p.QuantidadeEstoque,
                    GrupoNome = p.GrupoProduto.Nome
                })
                .ToList();

            return View(fatura);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Fatura fatura, int clienteId, int[] produtoIds, int[] quantidades)
        {
            if (id != fatura.ID) return NotFound();

            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null)
            {
                ModelState.AddModelError("Cliente", "Cliente não encontrado.");
            }
            else
            {
                fatura.Cliente = cliente;
                fatura.ClienteID = clienteId;
            }

            var faturaDb = await _context.Faturas
                .Include(f => f.FaturaProdutos)
                .FirstOrDefaultAsync(f => f.ID == id);

            if (faturaDb == null) return NotFound();

            // Devolver estoque anterior
            foreach (var faturaProduto in faturaDb.FaturaProdutos)
            {
                var produtoAntigo = await _context.Produtos.FindAsync(faturaProduto.ProdutoID);
                if (produtoAntigo != null)
                    produtoAntigo.QuantidadeEstoque += faturaProduto.Quantidade;
            }

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

                            produto.QuantidadeEstoque -= quantidades[i];
                        }
                    }
                }
            }

            if (!ModelState.IsValid || hasStockError)
            {
                ViewBag.Clientes = _context.Clientes.ToList();
                ViewBag.Produtos = _context.Produtos
                    .Select(p => new
                    {
                        p.ID,
                        p.Descricao,
                        p.Preco,
                        Estoque = p.QuantidadeEstoque,
                        GrupoNome = p.GrupoProduto.Nome
                    })
                    .ToList();
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var fatura = await _context.Faturas
                .Include(f => f.Cliente)
                .FirstOrDefaultAsync(f => f.ID == id);

            if (fatura == null) return NotFound();

            return View(fatura);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fatura = await _context.Faturas
                .Include(f => f.FaturaProdutos)
                .FirstOrDefaultAsync(f => f.ID == id);

            if (fatura != null)
            {
                // Retornar os produtos para o estoque
                foreach (var faturaProduto in fatura.FaturaProdutos)
                {
                    var produto = await _context.Produtos.FindAsync(faturaProduto.ProdutoID);
                    if (produto != null)
                    {
                        produto.QuantidadeEstoque += faturaProduto.Quantidade;
                    }
                }

                _context.FaturaProdutos.RemoveRange(fatura.FaturaProdutos);
                _context.Faturas.Remove(fatura);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ListaDeFaturas));
        }
    }
}