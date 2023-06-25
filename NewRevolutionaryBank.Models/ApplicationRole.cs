namespace NewRevolutionaryBank.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

[Comment("Потребителска роля")]
public class ApplicationRole : IdentityRole<Guid>
{
	public ApplicationRole(string name)
		: base(name) => CreatedOn = DateTime.UtcNow;

	[Comment("Дата на създаване на ролята")]
	public DateTime CreatedOn { get; set; }
}
