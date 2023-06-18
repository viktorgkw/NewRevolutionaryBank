﻿namespace NewRevolutionaryBank.Models;

using Microsoft.AspNetCore.Identity;

public class ApplicationRole : IdentityRole
{
	public ApplicationRole(string name)
		: base(name)
	{
		CreatedOn = DateTime.UtcNow;
	}

	public DateTime CreatedOn { get; set; }
}
