namespace NewRevolutionaryBank.Data.Models;

using System.ComponentModel.DataAnnotations;

public class Rating
{
	public Rating() => Id = Guid.NewGuid();

	[Key]
    public Guid Id { get; set; }

    public int RateValue { get; set; }

    [Required]
    public ApplicationUser RatedBy { get; set; } = null!;
}
