using Microsoft.EntityFrameworkCore;
using Smartuser.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURAR A CONEX�O COM O BANCO DE DADOS LOCAL
builder.Services.AddDbContext<SmartuserContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// CONFIGURAR JSON PARA IGNORAR CICLOS + AUTORIZA��O GLOBAL
builder.Services.AddControllersWithViews(options =>
{
    // Aplica [Authorize] em todo o sistema (exceto onde tiver [AllowAnonymous])
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

// AUTENTICA��O VIA COOKIE
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Login";            // Rota para login
        options.AccessDeniedPath = "/AcessoNegado"; // (opcional) p�gina para acesso negado
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// PERMITIR USO DE ARQUIVOS EST�TICOS (ex: logo, css)
app.UseStaticFiles();

app.UseRouting();

// ATIVAR AUTENTICA��O E AUTORIZA��O
app.UseAuthentication();
app.UseAuthorization();

// ROTA PADR�O
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
