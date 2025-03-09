using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using Smartuser.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Smartuser.Controllers
{
    public class ItemController : Controller
    {
        private readonly SmartuserContext _context;

        public ItemController(SmartuserContext context)
        {
            _context = context;
        }

        // AÇÃO PARA EXIBIR A LISTA DE ESTOQUE (ListaDeEstoque)
        public async Task<IActionResult> ListaDeEstoque()
        {
            var itens = await _context.Itens
                                      .Include(i => i.Grupo)  // Inclui o Grupo para acessar seu nome
                                      .ToListAsync();
            return View(itens);
        }

        // AÇÃO PARA EXIBIR A TELA DE CRIAÇÃO DE ITEM (Criar)
        public IActionResult Criar()
        {
            // Passa os grupos para o dropdown
            ViewBag.Grupos = _context.Grupos.ToList();
            return View();
        }

        // AÇÃO PARA CRIAR O ITEM (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Item item)
        {
            // Recarrega os grupos para caso seja necessário retornar a view com erros
            ViewBag.Grupos = _context.Grupos.ToList();

            if (!ModelState.IsValid)
                return View(item);

            // Recupera o valor do Grupo selecionado no formulário
            var grupoIdStr = Request.Form["GrupoID"].ToString();
            if (string.IsNullOrEmpty(grupoIdStr) || !int.TryParse(grupoIdStr, out int grupoID))
            {
                ModelState.AddModelError("GrupoID", "Selecione um grupo válido.");
                return View(item);
            }

            var grupo = await _context.Grupos.FindAsync(grupoID);
            if (grupo == null)
            {
                ModelState.AddModelError("GrupoID", "Grupo não encontrado.");
                return View(item);
            }

            // Associa o grupo encontrado ao item
            item.Grupo = grupo;
            item.DataCriacao = DateTime.Now;
            _context.Itens.Add(item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Registre o erro conforme sua estratégia (aqui, adicionamos um ModelError)
                ModelState.AddModelError("", "Erro ao salvar o item: " + ex.Message);
                return View(item);
            }

            // Após salvar com sucesso, redireciona para ListaDeEstoque
            return RedirectToAction(nameof(ListaDeEstoque));
        }

        // AÇÃO PARA EXIBIR A TELA DE EDIÇÃO DE ITEM (GET Editar)
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _context.Itens
                                     .Include(i => i.Grupo)
                                     .FirstOrDefaultAsync(i => i.ID == id);
            if (item == null)
                return NotFound();

            ViewBag.Grupos = _context.Grupos.ToList();
            return View(item);
        }

        // AÇÃO PARA ATUALIZAR O ITEM (POST Editar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Item item)
        {
            if (id != item.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var itemNoBanco = await _context.Itens.FindAsync(id);
                    if (itemNoBanco == null)
                        return NotFound();

                    itemNoBanco.Descricao = item.Descricao;

                    // Recupera o grupo selecionado do formulário
                    var grupoIdStr = Request.Form["GrupoID"].ToString();
                    if (!string.IsNullOrEmpty(grupoIdStr) && int.TryParse(grupoIdStr, out int grupoID))
                    {
                        var grupo = await _context.Grupos.FindAsync(grupoID);
                        if (grupo != null)
                        {
                            itemNoBanco.Grupo = grupo;
                        }
                        else
                        {
                            ModelState.AddModelError("GrupoID", "Grupo não encontrado.");
                            ViewBag.Grupos = _context.Grupos.ToList();
                            return View(item);
                        }
                    }

                    itemNoBanco.QuantidadeEstoque = item.QuantidadeEstoque;
                    itemNoBanco.Preco = item.Preco;
                    itemNoBanco.DataUltimaAtualizacao = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Itens.Any(e => e.ID == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(ListaDeEstoque));
            }
            ViewBag.Grupos = _context.Grupos.ToList();
            return View(item);
        }

        // AÇÃO PARA EXIBIR A TELA DE CONFIRMAÇÃO DE EXCLUSÃO (GET Delete)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _context.Itens
                                     .Include(i => i.Grupo)
                                     .FirstOrDefaultAsync(m => m.ID == id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // AÇÃO PARA DELETAR O ITEM (POST Delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Itens.FindAsync(id);
            if (item != null)
            {
                _context.Itens.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListaDeEstoque));
        }

        // AÇÃO PARA CRIAR NOVO GRUPO (Chamada pelo modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarGrupo(Grupo grupo)
        {
            if (ModelState.IsValid)
            {
                _context.Grupos.Add(grupo);
                await _context.SaveChangesAsync();
                // Após criar o grupo, redireciona para a tela de criação de Item
                return RedirectToAction(nameof(Criar));
            }
            // Se a validação falhar, retorna o modal com os erros
            return PartialView("_CreateGrupoModal", grupo);
        }
    }
}
