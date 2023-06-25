namespace NewRevolutionaryBank.Web.Middlewares;

using NewRevolutionaryBank.Services.Contracts;

public class IsDeletedMiddleware
{
	private readonly RequestDelegate _next;

	public IsDeletedMiddleware(RequestDelegate next) => _next = next;

	public async Task InvokeAsync(HttpContext context, IMiddlewareService middlewareService)
	{
		if (context.User.Identity is null || !context.User.Identity.IsAuthenticated)
		{
			await _next(context);
			return;
		}

		bool mustLogout = await middlewareService
			.MustLogoutByUsernameAsync(context.User.Identity?.Name ?? string.Empty);

		if (mustLogout)
		{
			context.Response.Redirect("/Home/Index");
			return;
		}

		await _next(context);
	}
}
