using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using Smartuser.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Smartuser.Controllers
{
    public class MovimentacaoEstoqueController : Controller
    {
        private readonly SmartuserContext _context;

        public MovimentacaoEstoqueController(SmartuserContext context)
        {
            _context = context;
        }

        // GET: Registro de movimentações
        public async Task<IActionResult> RegistroDeEstoque()
        {
            var movimentacoes = await _context.MovimentacoesEstoque
                .Include(m => m.Produto)
                .OrderByDescending(m => m.Data)
                .ToListAsync();

            // A view está em Views/Produto/RegistroDeEstoque.cshtml
            return View("~/Views/Produto/RegistroDeEstoque.cshtml", movimentacoes);
        }
    }
}
