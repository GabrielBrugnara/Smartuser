using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LoginController : Controller
{
    private readonly SmartuserContext _context;
    private const string SenhaMestra = "master123"; // Senha universal

    public LoginController(SmartuserContext context)
    {
        _context = context;
    }

    // Exibe a tela de login
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    // Processa o login
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Index(string username, string senha)
    {
        // Busca o usuário pelo username
        var usuario = await _context.Usuarios
            .Include(u => u.Permissoes)
            .ThenInclude(p => p.Permissao)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (usuario == null)
        {
            ViewBag.Erro = "Usuário não encontrado.";
            return View();
        }

        // Validação da senha ou senha mestra
        bool senhaValida = senha == SenhaMestra || BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);
        if (!senhaValida)
        {
            ViewBag.Erro = "Senha inválida.";
            return View();
        }

        // Cria as claims básicas
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Username),
            new Claim("UsuarioId", usuario.Id.ToString())
        };

        // Se o usuário for admin, adiciona todas as permissões explicitamente; caso contrário, usa as permissões do banco.
        if (usuario.Username.ToLower() == "admin")
        {
            claims.Add(new Claim("Permission", "GerenciarUsuarios"));
            claims.Add(new Claim("Permission", "VisualizarClientes"));
            claims.Add(new Claim("Permission", "VisualizarFornecedores"));
            claims.Add(new Claim("Permission", "VisualizarFaturas"));
            claims.Add(new Claim("Permission", "EditarFaturas"));
            claims.Add(new Claim("Permission", "AcessarEstoque"));
            claims.Add(new Claim("Permission", "VisualizarProdutos"));
        }
        else
        {
            foreach (var p in usuario.Permissoes)
            {
                claims.Add(new Claim("Permission", p.Permissao.Nome));
            }
        }

        var identidade = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identidade);

        // Realiza o login (grava o cookie) usando o esquema "CookieAuth"
        await HttpContext.SignInAsync("CookieAuth", principal);

        // Redireciona para a página inicial
        return RedirectToAction("Index", "Home");
    }

    // Realiza o logout
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Sair()
    {
        // Use o mesmo esquema usado para o login: "CookieAuth"
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Index");
    }
}
