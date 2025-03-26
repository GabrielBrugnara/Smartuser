using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using Smartuser.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Smartuser.Controllers
{
    public class GrupoFornecedorController : Controller
    {
        private readonly SmartuserContext _context;

        public GrupoFornecedorController(SmartuserContext context)
        {
            _context = context;
        }

        // GET: GrupoFornecedor/ListaGrupos
        public async Task<IActionResult> ListaGrupos()
        {
            var grupos = await _context.GrupoFornecedores.ToListAsync();
            return View(grupos);
        }

        // GET: GrupoFornecedor/Criar
        public IActionResult Criar()
        {
            return View();
        }

        // POST: GrupoFornecedor/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(GrupoFornecedor grupo)
        {
            if (!ModelState.IsValid)
                return View(grupo);

            _context.GrupoFornecedores.Add(grupo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListaGrupos));
        }

        // GET: GrupoFornecedor/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var grupo = await _context.GrupoFornecedores.FindAsync(id);
            if (grupo == null)
                return NotFound();

            return View(grupo);
        }

        // POST: GrupoFornecedor/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, GrupoFornecedor grupo)
        {
            if (id != grupo.ID)
                return NotFound();

            if (!ModelState.IsValid)
                return View(grupo);

            _context.Update(grupo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListaGrupos));
        }

        // GET: GrupoFornecedor/Deletar/5
        public async Task<IActionResult> Deletar(int? id)
        {
            if (id == null)
                return NotFound();

            var grupo = await _context.GrupoFornecedores.FindAsync(id);
            if (grupo == null)
                return NotFound();

            return View(grupo);
        }

        // POST: GrupoFornecedor/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletarConfirmado(int id)
        {
            var grupo = await _context.GrupoFornecedores.FindAsync(id);
            if (grupo != null)
            {
                _context.GrupoFornecedores.Remove(grupo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ListaGrupos));
        }
    }
}
