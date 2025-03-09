using Microsoft.AspNetCore.Mvc;
using Smartuser.Models;
using Smartuser.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Smartuser.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly SmartuserContext _context;

        public UsuarioController(SmartuserContext context)
        {
            _context = context;
        }

        // LISTAR USUÁRIOS
        public IActionResult Index()
        {
            var usuarios = _context.Usuarios.ToList();
            return View("ListaUsuarios", usuarios);
        }

        // PÁGINA DE CRIAÇÃO
        public IActionResult Criar()
        {
            return View();
        }

        // PROCESSA O CADASTRO DO USUÁRIO
        [HttpPost]
        [ValidateAntiForgeryToken]  // Adicionando proteção contra ataques CSRF
        public async Task<IActionResult> Criar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.DataCriacao = DateTime.Now;
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();  // Usando SaveChangesAsync para operações assíncronas
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // PÁGINA DE EDIÇÃO
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);  // Usando FindAsync para operações assíncronas

            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // PROCESSA A EDIÇÃO
        [HttpPost]
        [ValidateAntiForgeryToken]  // Adicionando proteção contra ataques CSRF
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioExistente = await _context.Usuarios.FindAsync(usuario.ID);  // Usando FindAsync para operações assíncronas

                if (usuarioExistente == null)
                {
                    return NotFound();
                }

                usuarioExistente.Nome = usuario.Nome;
                usuarioExistente.Sobrenome = usuario.Sobrenome;
                usuarioExistente.Email = usuario.Email;
                usuarioExistente.UltimaAlteracao = DateTime.Now;

                await _context.SaveChangesAsync();  // Usando SaveChangesAsync para operações assíncronas
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // PROCESSA A EXCLUSÃO
        [HttpPost]
        [ValidateAntiForgeryToken]  // Adicionando proteção contra ataques CSRF
        public async Task<IActionResult> Deletar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);  // Usando FindAsync para operações assíncronas

            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();  // Usando SaveChangesAsync para operações assíncronas
            return RedirectToAction("Index");
        }
    }
}
