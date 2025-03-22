using Microsoft.EntityFrameworkCore;
using Smartuser.Data;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURAR A CONEXÃO COM O BANCO DE DADOS LOCAL (USANDO STRING DE CONEXÃO DO appsettings.json)
builder.Services.AddDbContext<SmartuserContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// CONFIGURAR JSON PARA IGNORAR CICLOS
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

// PERMITIR O USO DE ARQUIVOS ESTÁTICOS (PARA A LOGO E OUTROS RECURSOS)
app.UseStaticFiles();

// CONFIGURAR O ROTEAMENTO
app.UseRouting();

// CONFIGURAR A ROTA PADRÃO PARA O "Home" COMO CONTROLADOR PRINCIPAL
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
