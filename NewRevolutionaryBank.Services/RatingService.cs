namespace NewRevolutionaryBank.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;

public class RatingService : IRatingService
{
	private readonly NrbDbContext _context;

	public RatingService(NrbDbContext context) => _context = context;

	/// <returns>Returns all the website ratings.</returns>
	public async Task<List<Rating>> GetAll() =>
		await _context.Ratings
			.Include(r => r.RatedBy)
			.ToListAsync();

	/// <returns>True or false if the user has already rated the website.</returns>
	public async Task<bool> HasAlreadyRated(string? userName) =>
		await _context.Ratings.AnyAsync(r => r.RatedBy.UserName == userName);

	/// <summary>
	/// Saves the sent rating to the database.
	/// </summary>
	public async Task SendRating(Rating model)
	{
		await _context.Ratings.AddAsync(model);

		await _context.SaveChangesAsync();
	}
}
