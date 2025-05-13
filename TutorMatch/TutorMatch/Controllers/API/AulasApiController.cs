using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TutorMatch.Data;
using TutorMatch.Models;

namespace TutorMatch.Controllers.Api
	{
	[Route("api/aulas")]
	[ApiController]
	[Authorize]
	public class AulasApiController : ControllerBase
		{
		private readonly ApplicationDbContext _context;

		public AulasApiController(ApplicationDbContext context)
			{
			_context = context;
			}

		// GET: api/aulas
		[HttpGet]
		public async Task<IActionResult> GetAulas()
			{
			var aulas = await _context.Aulas
				.Include(a => a.Professor)
				.ToListAsync();
			return Ok(aulas);
			}

		// GET: api/aulas/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetAula(int id)
			{
			var aula = await _context.Aulas
				.Include(a => a.Professor)
				.FirstOrDefaultAsync(a => a.Id == id);

			if (aula == null)
				{
				return NotFound();
				}

			return Ok(aula);
			}

		// POST: api/aulas
		[HttpPost]
		public async Task<IActionResult> CreateAula([FromBody] Aula aula)
			{
			var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(professorId))
				{
				return Unauthorized(new { message = "Usuário não autenticado" });
				}

			aula.ProfessorId = professorId;

			if (!ModelState.IsValid)
				{
				return BadRequest(ModelState);
				}

			_context.Aulas.Add(aula);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAula), new { id = aula.Id }, aula);
			}

		// PUT: api/aulas/5
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAula(int id, [FromBody] Aula aula)
			{
			if (id != aula.Id)
				{
				return BadRequest(new { message = "ID da aula não corresponde" });
				}

			var existingAula = await _context.Aulas.FindAsync(id);
			if (existingAula == null)
				{
				return NotFound();
				}

			var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (existingAula.ProfessorId != professorId)
				{
				return Forbid();
				}

			if (!ModelState.IsValid)
				{
				return BadRequest(ModelState);
				}

			_context.Entry(existingAula).CurrentValues.SetValues(aula);

			try
				{
				await _context.SaveChangesAsync();
				}
			catch (DbUpdateConcurrencyException)
				{
				if (!AulaExists(id))
					{
					return NotFound();
					}
				else
					{
					throw;
					}
				}

			return NoContent();
			}

		// DELETE: api/aulas/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAula(int id)
			{
			var aula = await _context.Aulas.FindAsync(id);
			if (aula == null)
				{
				return NotFound();
				}

			var professorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (aula.ProfessorId != professorId)
				{
				return Forbid();
				}

			_context.Aulas.Remove(aula);
			await _context.SaveChangesAsync();

			return NoContent();
			}

		private bool AulaExists(int id)
			{
			return _context.Aulas.Any(e => e.Id == id);
			}
		}
	}