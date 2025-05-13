using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TutorMatch.Data;
using TutorMatch.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models; // Adicionado para o Swagger

namespace TutorMatch;

public static class Program
	{
	public static async Task Main(string[] args)
		{
		var builder = WebApplication.CreateBuilder(args);

		// Adicionar suporte para variáveis de ambiente (original)
		builder.Configuration
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables();

		// Recuperar a connection string (original)
		var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
			?? $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
			   $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
			   $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
			   $"Database={Environment.GetEnvironmentVariable("DB_NAME")};";

		if (string.IsNullOrEmpty(connectionString))
			{
			throw new InvalidOperationException("Connection string 'DefaultConnection' or environment variable 'DB_CONNECTION_STRING' not found.");
			}

		// Configurar o contexto de banco de dados (original)
		builder.Services.AddDbContext<ApplicationDbContext>(options =>
			options.UseNpgsql(connectionString));

		builder.Services.AddDatabaseDeveloperPageExceptionFilter();

		// Configurar o Identity (original)
		builder.Services.AddIdentity<User, IdentityRole>(options =>
		{
			options.Password.RequireDigit = true;
			options.Password.RequiredLength = 6;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireUppercase = false;
			options.Password.RequireLowercase = true;
			options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
			options.Lockout.MaxFailedAccessAttempts = 5;
			options.Lockout.AllowedForNewUsers = true;
			options.User.RequireUniqueEmail = true;
		})
		.AddEntityFrameworkStores<ApplicationDbContext>()
		.AddDefaultTokenProviders();

		// Configurar autenticação via cookie (original)
		builder.Services.ConfigureApplicationCookie(options =>
		{
			options.LoginPath = "/Account/Login";
			options.AccessDeniedPath = "/Account/AccessDenied";
		});

		// Serviços para Razor Pages e MVC (original)
		builder.Services.AddRazorPages();
		builder.Services.AddControllersWithViews();

		// ============= ADIÇÕES PARA O SWAGGER =============
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
				{
				Title = "TutorMatch API",
				Version = "v1"
				});
		});

		var app = builder.Build();

		// Pipeline de requisições HTTP (original)
		if (app.Environment.IsDevelopment())
			{
			app.UseMigrationsEndPoint();
			// ============= ADIÇÕES PARA O SWAGGER =============
			app.UseSwagger();
			app.UseSwaggerUI();
			}
		else
			{
			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();
			}

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();

		// Rotas originais (mantidas intactas)
		app.MapRazorPages();
		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");

		app.MapControllerRoute(
			name: "dbTest",
			pattern: "DatabaseTest",
			defaults: new { controller = "DatabaseTest", action = "Index" });

		await app.RunAsync();
		}
	}