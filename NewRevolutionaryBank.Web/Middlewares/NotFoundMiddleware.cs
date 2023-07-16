namespace NewRevolutionaryBank.Web.Middlewares;

using NewRevolutionaryBank.Web.Handlers;

public class NotFoundMiddleware
{
	private readonly RequestDelegate _next;

	public NotFoundMiddleware(RequestDelegate next) => _next = next;

	public async Task InvokeAsync(HttpContext context)
	{
		await _next(context);

		if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
		{
			string title = "404";
			string description = "The page you were trying to access does not exist!";
			bool isNotFound = true;

			string errorUrl = $"/Home/Error?title={Uri.EscapeDataString(title)}&description={Uri.EscapeDataString(description)}&isNotFound={isNotFound}";

			context.Response.Redirect(errorUrl);
		}
	}
}
