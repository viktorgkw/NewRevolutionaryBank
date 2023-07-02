namespace NewRevolutionaryBank.Data.Models;

using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

[Comment("Рейтинг на уебсайта")]
public class Rating
{
	public Rating() => Id = Guid.NewGuid();

	[Key]
	[Comment("Уникален идентификатор")]
	public Guid Id { get; set; }

	[Comment("Стойност на рейтинга")]
	public int RateValue { get; set; }

    [Required]
	[Comment("Потребителя, който е оценил")]
	public ApplicationUser RatedBy { get; set; } = null!;
}
