using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using Smartuser.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Smartuser.Controllers
{
    /// <summary>
    /// Controller para gerenciar as ações relacionadas aos itens (estoque) do sistema.
    /// </summary>
    public class ItemController : Controller
    {
        private readonly SmartuserContext _context;

        /// <summary>
        /// Injeta o contexto do banco de dados.
        /// </summary>
        /// <param name="context">Instância do SmartuserContext.</param>
        public ItemController(SmartuserContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Exibe a lista de itens em estoque.
        /// </summary>
        /// <returns>View com a lista de itens, incluindo o Grupo associado.</returns>
        public async Task<IActionResult> ListaDeEstoque()
        {
            // Inclui o Grupo associado para exibir seu nome na view
            var itens = await _context.Itens
                                      .Include(i => i.Grupo)
                                      .ToListAsync();
            return View(itens);
        }

        /// <summary>
        /// Exibe a tela de criação de item.
        /// </summary>
        /// <returns>View de criação de item com a lista de grupos disponíveis.</returns>
        public IActionResult Criar()
        {
            // Passa os grupos para o dropdown na view
            ViewBag.Grupos = _context.Grupos.ToList();
            return View();
        }

        /// <summary>
        /// Processa a criação de um novo item (ação POST).
        /// </summary>
        /// <param name="item">Objeto Item recebido do formulário.</param>
        /// <returns>Redireciona para ListaDeEstoque se a criação for bem-sucedida; caso contrário, retorna a view com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Item item)
        {
            // Recarrega os grupos para manter o dropdown em caso de erro de validação
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

            // Busca o grupo correspondente ao ID informado
            var grupo = await _context.Grupos.FindAsync(grupoID);
            if (grupo == null)
            {
                ModelState.AddModelError("GrupoID", "Grupo não encontrado.");
                return View(item);
            }

            // Associa o grupo ao item e define a data de criação
            item.Grupo = grupo;
            item.DataCriacao = DateTime.Now;
            _context.Itens.Add(item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Em caso de erro, adiciona a mensagem de exceção ao ModelState
                ModelState.AddModelError("", "Erro ao salvar o item: " + ex.Message);
                return View(item);
            }

            // Redireciona para a lista de estoque após a criação bem-sucedida
            return RedirectToAction(nameof(ListaDeEstoque));
        }

        /// <summary>
        /// Exibe a tela de edição de um item.
        /// </summary>
        /// <param name="id">ID do item a ser editado.</param>
        /// <returns>View de edição com os dados atuais do item e a lista de grupos.</returns>
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            // Busca o item e inclui o Grupo associado
            var item = await _context.Itens
                                     .Include(i => i.Grupo)
                                     .FirstOrDefaultAsync(i => i.ID == id);
            if (item == null)
                return NotFound();

            // Passa os grupos para o dropdown da view de edição
            ViewBag.Grupos = _context.Grupos.ToList();
            return View(item);
        }

        /// <summary>
        /// Processa a atualização dos dados de um item (ação POST para Editar).
        /// </summary>
        /// <param name="id">ID do item a ser atualizado.</param>
        /// <param name="item">Objeto Item com os dados atualizados.</param>
        /// <returns>Redireciona para ListaDeEstoque se a atualização for bem-sucedida; caso contrário, retorna a view com erros.</returns>
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
                    // Busca o item existente no banco de dados
                    var itemNoBanco = await _context.Itens.FindAsync(id);
                    if (itemNoBanco == null)
                        return NotFound();

                    // Atualiza as propriedades básicas do item
                    itemNoBanco.Descricao = item.Descricao;

                    // Recupera o grupo selecionado no formulário
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

                    // Atualiza os demais campos do item
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
            // Em caso de falha na validação, recarrega os grupos e retorna a view de edição
            ViewBag.Grupos = _context.Grupos.ToList();
            return View(item);
        }

        /// <summary>
        /// Exibe a tela de confirmação de exclusão de um item.
        /// </summary>
        /// <param name="id">ID do item a ser excluído.</param>
        /// <returns>View de confirmação para exclusão do item.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            // Busca o item e inclui o Grupo associado para exibição na view
            var item = await _context.Itens
                                     .Include(i => i.Grupo)
                                     .FirstOrDefaultAsync(m => m.ID == id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        /// <summary>
        /// Processa a exclusão de um item (ação POST para Delete).
        /// </summary>
        /// <param name="id">ID do item a ser excluído.</param>
        /// <returns>Redireciona para ListaDeEstoque após a exclusão.</returns>
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

        /// <summary>
        /// Cria um novo grupo a partir de um modal (ação POST para CriarGrupo).
        /// </summary>
        /// <param name="grupo">Objeto Grupo recebido do formulário.</param>
        /// <returns>Redireciona para a tela de criação de item ou retorna o modal com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarGrupo(Grupo grupo)
        {
            if (ModelState.IsValid)
            {
                _context.Grupos.Add(grupo);
                await _context.SaveChangesAsync();
                // Após criar o grupo, redireciona para a tela de criação de item
                return RedirectToAction(nameof(Criar));
            }
            // Se a validação falhar, retorna o modal com os erros
            return PartialView("_CreateGrupoModal", grupo);
        }
    }
}
