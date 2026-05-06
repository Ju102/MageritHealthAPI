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
// Se obtiene la URI de Key Vault desde la configuración
var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
if (!string.IsNullOrEmpty(keyVaultUri))
{
    // Se integran los secretos de Key Vault en la configuración de la aplicación
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        new DefaultAzureCredential());
}

// Se añaden los servicios base al contenedor de dependencias
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// ================================================================
// HELPERS Y SERVICIOS
// ================================================================

// Se instancia y registra el helper de OAuth como Singleton
HelperActionOAuthService authHelper = new HelperActionOAuthService(builder.Configuration);
builder.Services.AddSingleton<HelperActionOAuthService>(authHelper);

// Se inicializa el componente de cifrado con la configuración actual
CifradoHelper.Initialize(builder.Configuration);

// Se registran los helpers de seguridad y herramientas
builder.Services.AddSingleton<CryptographyHelper>();
builder.Services.AddSingleton<ToolsHelper>();
builder.Services.AddSingleton<UserTokenHelper>();

// Se inyectan los servicios de lógica de negocio con ciclo de vida Transient
builder.Services.AddTransient<IEmailingService, EmailingService>();
builder.Services.AddTransient<IExportService, ExportService>();
builder.Services.AddTransient<SimuladorLaboratorioService>();
builder.Services.AddTransient<IAzureBlobService, AzureBlobService>();

// ================================================================
// AUTENTICACIÓN Y BASE DE DATOS
// ================================================================
// Se configura el esquema de autenticación y el soporte para JWT
builder.Services.AddAuthentication(authHelper.GetAuthenticationSchema())
    .AddJwtBearer(authHelper.GetJwtBearerOptions());

// Se recupera la cadena de conexión de la base de datos
string connectionString = builder.Configuration.GetConnectionString("MageritHealthConnection");

// Se establece el contexto de la base de datos con SQL Server
builder.Services.AddDbContext<MageritHealthDbContext>(options =>
    options.UseSqlServer(connectionString));

// Se registran los repositorios para el acceso a datos
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

// Se habilitan los controladores y la generación de OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ================================================================
// PIPELINE DE LA APLICACIÓN (MIDDLEWARE)
// ================================================================
// Se exponen los endpoints para la documentación de la API
app.MapOpenApi();
app.MapScalarApiReference();

// Se define la redirección de la ruta raíz hacia la documentación
app.MapGet("/", context =>
{
    context.Response.Redirect("/scalar");
    return Task.CompletedTask;
});

// Se configuran los middlewares del ciclo de vida de la solicitud
app.UseSession();
app.UseHttpsRedirection();

// Se activan los mecanismos de seguridad (Autenticación y Autorización)
app.UseAuthentication();
app.UseAuthorization();

// Se mapean las rutas de los controladores
app.MapControllers();

// Se inicia la ejecución de la aplicación
app.Run();