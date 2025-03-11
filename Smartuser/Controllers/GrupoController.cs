using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using Smartuser.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Smartuser.Controllers
{
    // Controller para gerenciar grupos
    public class GrupoController : Controller
    {
        private readonly SmartuserContext _context;

        public GrupoController(SmartuserContext context)
        {
            _context = context;
        }

        // Lista os grupos cadastrados
        public async Task<IActionResult> ListaGrupos()
        {
            var grupos = await _context.Grupos.ToListAsync();
            return View(grupos);
        }

        // Exibe o formulário para criar um novo grupo
        public IActionResult Criar()
        {
            return View();
        }

        // Cria um novo grupo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Grupo grupo)
        {
            if (ModelState.IsValid)
            {
                _context.Grupos.Add(grupo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListaGrupos));
            }
            return View(grupo);
        }

        // Exibe o formulário para editar um grupo existente
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var grupo = await _context.Grupos.FindAsync(id);
            if (grupo == null)
                return NotFound();

            return View(grupo);
        }

        // Processa a edição de um grupo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Grupo grupo)
        {
            if (id != grupo.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grupo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Grupos.Any(e => e.ID == grupo.ID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(ListaGrupos));
            }
            return View(grupo);
        }

        // Exibe a tela de confirmação para exclusão de um grupo
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var grupo = await _context.Grupos.FirstOrDefaultAsync(m => m.ID == id);
            if (grupo == null)
                return NotFound();

            return View(grupo);
        }

        // Exclui o grupo; se ocorrer erro (ex.: itens associados), redireciona com mensagem
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grupo = await _context.Grupos.FindAsync(id);
            if (grupo != null)
            {
                _context.Grupos.Remove(grupo);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "Não foi possível excluir o grupo, pois existem itens associados.";
                    return RedirectToAction(nameof(ListaGrupos));
                }
            }
            return RedirectToAction(nameof(ListaGrupos));
        }
    }
}
