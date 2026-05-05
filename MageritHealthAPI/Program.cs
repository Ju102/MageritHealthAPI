using Azure.Identity;
using MageritHealth.Services;
using MageritHealthAPI.Data;
using MageritHealthAPI.Helpers;
using MageritHealthAPI.Repositories;
using MageritHealthAPI.Repositories.Interfaces;
using MageritHealthAPI.Services;
using MageritHealthAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ================================================================
// INTEGRACIÓN CON AZURE KEY VAULT
// ================================================================
var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
if (!string.IsNullOrEmpty(keyVaultUri))
{
    // Inyección de los Secrets de Key Vault en builder.Configuration
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        new DefaultAzureCredential());
}

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// ================================================================
// HELPERS Y SERVICIOS
// ================================================================

// Instancia del Helper de OAuth
HelperActionOAuthService authHelper = new HelperActionOAuthService(builder.Configuration);
builder.Services.AddSingleton<HelperActionOAuthService>(authHelper);

// Inicializacion del Helper de cifrado
CifradoHelper.Initialize(builder.Configuration);

// Instancia de Helpers de Salts y cifrado de passwords
builder.Services.AddSingleton<CryptographyHelper>();
builder.Services.AddSingleton<ToolsHelper>();
builder.Services.AddSingleton<UserTokenHelper>();

builder.Services.AddTransient<IEmailingService, EmailingService>();
builder.Services.AddTransient<SimuladorLaboratorioService>();

builder.Services.AddTransient<IAzureBlobService, AzureBlobService>();
builder.Services.AddTransient<ExportService>();

// ================================================================
// AUTENTICACIÓN Y BASE DE DATOS
// ================================================================
builder.Services.AddAuthentication(authHelper.GetAuthenticationSchema())
    .AddJwtBearer(authHelper.GetJwtBearerOptions());

// Leer la cadena de conexión
string connectionString = builder.Configuration.GetConnectionString("MageritHealthConnection");
builder.Services.AddDbContext<MageritHealthDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddTransient<IAnaliticasRepository, AnaliticasRepository>();
builder.Services.AddTransient<IAntecedentesRepository, AntecedentesRepository>();
builder.Services.AddTransient<ICitasRepository, CitasRepository>();
builder.Services.AddTransient<IEspecialidadesRepository, EspecialidadesRepository>();
builder.Services.AddTransient<IInfoClinicaRepository, InfoClinicaRepository>();
builder.Services.AddTransient<IMedicamentosRepository, MedicamentosRepository>();
builder.Services.AddTransient<INotificacionesRepository, NotificacionesRepository>();
builder.Services.AddTransient<IPrescripcionesRepository, PrescripcionesRepository>();
builder.Services.AddTransient<ITiposMedicionRepository, TiposMedicionRepository>();
builder.Services.AddTransient<IUsuariosRepository, UsuariosRepository>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ================================================================
// PIPELINE DE LA APLICACIÓN
// ================================================================
app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", context =>
{
    context.Response.Redirect("/scalar");
    return Task.CompletedTask;
});

app.UseSession();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();