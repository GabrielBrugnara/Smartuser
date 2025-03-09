using Microsoft.EntityFrameworkCore;
using Smartuser.Data;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURAR A CONEXÃO COM O BANCO DE DADOS LOCAL (USANDO STRING DE CONEXÃO DO appsettings.json)
builder.Services.AddDbContext<SmartuserContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));  // LENDO A STRING DE CONEXÃO DO ARQUIVO

// ADICIONAR SERVIÇOS PARA CONTROLADORES E VIEWS
builder.Services.AddControllersWithViews();

var app = builder.Build();

// PERMITIR O USO DE ARQUIVOS ESTÁTICOS (PARA A LOGO E OUTROS RECURSOS)
app.UseStaticFiles();

// CONFIGURAR O ROTEAMENTO
app.UseRouting();

// CONFIGURAR A ROTA PADRÃO PARA O "Home" COMO CONTROLADOR PRINCIPAL
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");  // ALTERADO PARA USAR "Dashboard" POR PADRÃO

app.Run();
