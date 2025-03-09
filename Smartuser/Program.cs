using Microsoft.EntityFrameworkCore;
using Smartuser.Data;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURAR A CONEX�O COM O BANCO DE DADOS LOCAL (USANDO STRING DE CONEX�O DO appsettings.json)
builder.Services.AddDbContext<SmartuserContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));  // LENDO A STRING DE CONEX�O DO ARQUIVO

// ADICIONAR SERVI�OS PARA CONTROLADORES E VIEWS
builder.Services.AddControllersWithViews();

var app = builder.Build();

// PERMITIR O USO DE ARQUIVOS EST�TICOS (PARA A LOGO E OUTROS RECURSOS)
app.UseStaticFiles();

// CONFIGURAR O ROTEAMENTO
app.UseRouting();

// CONFIGURAR A ROTA PADR�O PARA O "Home" COMO CONTROLADOR PRINCIPAL
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");  // ALTERADO PARA USAR "Dashboard" POR PADR�O

app.Run();
