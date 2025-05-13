using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TutorMatch.Models;

namespace TutorMatch.Models
	{
	public class InscricaoAula
		{
		[Key]
		public int Id { get; set; }

		[Required]
		public required string UserId { get; set; }

		[ForeignKey("UserId")]
		public User User { get; set; } = null!;

		[Required]
		public required int AulaId { get; set; }

		[ForeignKey("AulaId")]
		public Aula Aula { get; set; } = null!;

		[Required(ErrorMessage = "A data de inscrição é obrigatória.")]
		public DateTime DataInscricao { get; set; }
		}
	}
