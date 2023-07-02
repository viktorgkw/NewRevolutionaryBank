namespace NewRevolutionaryBank.Services.Contracts;

using System.Collections.Generic;

using NewRevolutionaryBank.Data.Models;

public interface IRatingService
{
	Task<List<Rating>> GetAll();

	Task<bool> HasAlreadyRated(string? userName);

	Task SendRating(Rating model);
}
