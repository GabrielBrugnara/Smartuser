using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using Smartuser.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Smartuser.Controllers
{
    public class ClienteController : Controller
    {
        private readonly SmartuserContext _context;

        public ClienteController(SmartuserContext context)
        {
            _context = context;
        }

        // Lista os clientes cadastrados
        public async Task<IActionResult> ListaClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return View(clientes);
        }

        // Exibe o formulário para criar um novo cliente
        public IActionResult Criar()
        {
            return View();
        }

        // Processa a criação de um novo cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListaClientes));
            }
            return View(cliente);
        }

        // Exibe o formulário para editar um cliente existente
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // Processa a edição de um cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Cliente cliente)
        {
            if (id != cliente.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Clientes.Any(e => e.ID == cliente.ID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(ListaClientes));
            }
            return View(cliente);
        }

        // Exibe a tela de confirmação para exclusão de um cliente
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.ID == id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // Processa a exclusão do cliente
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListaClientes));
        }
    }
}
