namespace NewRevolutionaryBank.Web.Extensions;

using System.Reflection;

using NewRevolutionaryBank.Services.Messaging;
using NewRevolutionaryBank.Services.Messaging.Contracts;
using NewRevolutionaryBank.Web.Handlers;
using NewRevolutionaryBank.Web.HangfireJobs;

public static class ConfigureServicesExtension
{
	public static void ServiceConfigurator(this IServiceCollection services)
	{
		Assembly? serviceAssembly = Assembly.Load("NewRevolutionaryBank.Services") ??
			throw new DllNotFoundException("NewRevolutionaryBank.Services is not found!");

		Type[] implementationTypes = serviceAssembly
			.GetTypes()
			.Where(t => t.Name.EndsWith("Service") && !t.IsInterface)
			.ToArray();

		foreach (Type implementationType in implementationTypes)
		{
			Type? interfaceType = implementationType
				.GetInterface($"I{implementationType.Name}") ??
				throw new InvalidOperationException(
					$"No interface is provided for the service with name: {implementationType.Name}");

			services.AddScoped(interfaceType, implementationType);
		}
	}

	public static void ConfigureOtherServices(this IServiceCollection services)
	{
		services.AddScoped<HangfireJobs>();
		services.AddScoped<IsDeletedHandler>();
		services.AddScoped<IEmailSender, MailKitEmailSender>();
	}
}
