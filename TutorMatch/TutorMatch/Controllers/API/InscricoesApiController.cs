using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TutorMatch.Data;
using TutorMatch.Models;

namespace TutorMatch.Controllers.Api // Namespace reflete a estrutura de pastas
	{
	[Route("api/inscricoes")] // Define a rota base da API
	[ApiController] // Indica que é um controlador de API
	[Authorize] // Opcional: mantém a autenticação
	public class InscricoesApiController : ControllerBase // Herda de ControllerBase (sem suporte a Views)
		{
		private readonly ApplicationDbContext _context;

		public InscricoesApiController(ApplicationDbContext context)
			{
			_context = context;
			}

		// GET: api/inscricoes
		[HttpGet]
		public async Task<IActionResult> GetInscricoes()
			{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var inscricoes = await _context.InscricoesAula
				.Include(i => i.Aula)
				.Where(i => i.UserId == userId)
				.ToListAsync();

			return Ok(inscricoes); // Retorna JSON
			}

		// POST: api/inscricoes
		[HttpPost]
		public async Task<IActionResult> Inscrever([FromBody] int aulaId) // Recebe o aulaId via corpo da requisição
			{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			// ... (mesma lógica do InscreverConfirmado, mas sem TempData/Redirect)
			return Ok(new { message = "Inscrição realizada com sucesso!" });
			}
		}
	}