using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using Smartuser.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Smartuser.Controllers
{
    public class GrupoProdutoController : Controller
    {
        private readonly SmartuserContext _context;

        public GrupoProdutoController(SmartuserContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ListaGrupos()
        {
            var grupos = await _context.GrupoProdutos.ToListAsync();
            return View(grupos);
        }

        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(GrupoProduto grupo)
        {
            if (ModelState.IsValid)
            {
                _context.GrupoProdutos.Add(grupo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListaGrupos));
            }
            return View(grupo);
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();

            var grupo = await _context.GrupoProdutos.FindAsync(id);
            if (grupo == null) return NotFound();

            return View(grupo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, GrupoProduto grupo)
        {
            if (id != grupo.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grupo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.GrupoProdutos.Any(e => e.ID == grupo.ID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(ListaGrupos));
            }
            return View(grupo);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var grupo = await _context.GrupoProdutos.FirstOrDefaultAsync(m => m.ID == id);
            if (grupo == null) return NotFound();

            return View(grupo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grupo = await _context.GrupoProdutos.FindAsync(id);
            if (grupo != null)
            {
                _context.GrupoProdutos.Remove(grupo);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "Não foi possível excluir o grupo, pois existem produtos associados.";
                    return RedirectToAction(nameof(ListaGrupos));
                }
            }
            return RedirectToAction(nameof(ListaGrupos));
        }

        // NOVO: Criar grupo via modal (AJAX)
        [HttpPost]
        public async Task<IActionResult> CriarViaModal([FromBody] GrupoProduto grupo)
        {
            if (string.IsNullOrWhiteSpace(grupo.Nome))
            {
                return BadRequest(new { success = false, message = "Nome do grupo é obrigatório." });
            }

            _context.GrupoProdutos.Add(grupo);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                grupo = new
                {
                    grupo.ID,
                    grupo.Nome
                }
            });
        }
    }
}
