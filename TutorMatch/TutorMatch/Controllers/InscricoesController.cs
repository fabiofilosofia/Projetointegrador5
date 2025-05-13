using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TutorMatch.Data;
using TutorMatch.Models;

namespace TutorMatch.Controllers
	{
	[Authorize]
	public class InscricoesController : Controller
		{
		private readonly ApplicationDbContext _context;

		public InscricoesController(ApplicationDbContext context)
			{
			_context = context;
			}

		// GET: Inscricoes/MinhasInscricoes
		// Lista todas as inscrições do usuário logado
		public async Task<IActionResult> MinhasInscricoes()
			{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var inscricoes = await _context.InscricoesAula
				.Include(i => i.Aula)
					.ThenInclude(a => a.Professor)
				.Where(i => i.UserId == userId)
				.ToListAsync();
			return View(inscricoes);
			}

		// GET: Inscricoes/Inscrever/{aulaId}
		// Exibe a confirmação para o usuário se inscrever em uma aula
		public async Task<IActionResult> Inscrever(int aulaId)
			{
			var aula = await _context.Aulas.FindAsync(aulaId);
			if (aula == null)
				{
				return NotFound();
				}
			ViewBag.Aula = aula;
			return View();
			}

		// POST: Inscricoes/Inscrever/{aulaId}
		// Processa a inscrição do usuário na aula
		[HttpPost, ActionName("Inscrever")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> InscreverConfirmado(int aulaId)
			{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// Verifica se o usuário já está inscrito na aula
			bool jaInscrito = await _context.InscricoesAula
				.AnyAsync(i => i.AulaId == aulaId && i.UserId == userId);
			if (!jaInscrito)
				{
				var inscricao = new InscricaoAula
					{
					AulaId = aulaId,
					UserId = userId,
					DataInscricao = DateTime.UtcNow // A data de inscrição em UTC
					};
				_context.InscricoesAula.Add(inscricao);
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "Inscrição realizada com sucesso!";
				}
			return RedirectToAction("MinhasInscricoes");
			}

		// GET: Inscricoes/Cancelar/{inscricaoId}
		// Exibe a confirmação para o cancelamento da inscrição
		public async Task<IActionResult> Cancelar(int? inscricaoId)
			{
			if (inscricaoId == null)
				{
				return NotFound();
				}
			var inscricao = await _context.InscricoesAula
				.Include(i => i.Aula)
					.ThenInclude(a => a.Professor)
				.FirstOrDefaultAsync(i => i.Id == inscricaoId);
			if (inscricao == null)
				{
				return NotFound();
				}
			return View(inscricao);
			}

		// POST: Inscricoes/Cancelar/{inscricaoId}
		// Processa o cancelamento da inscrição
		[HttpPost, ActionName("Cancelar")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CancelarConfirmado(int inscricaoId)
			{
			var inscricao = await _context.InscricoesAula.FindAsync(inscricaoId);
			if (inscricao != null)
				{
				_context.InscricoesAula.Remove(inscricao);
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "Inscrição cancelada com sucesso!";
				}
			return RedirectToAction("MinhasInscricoes");
			}
		}
	}
