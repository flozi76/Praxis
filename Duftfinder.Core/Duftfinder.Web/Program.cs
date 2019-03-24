using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Duftfinder
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			//var cert = new X509Certificate2("localhost.pfx", "duftfinder1");
			return WebHost.CreateDefaultBuilder(args)
				.UseKestrel(
					//options =>
					//{
					//	// Configure the Url and ports to bind to
					//	// This overrides calls to UseUrls and the ASPNETCORE_URLS environment variable, but will be 
					//	// overridden if you call UseIisIntegration() and host behind IIS/IIS Express
					//	options.Listen(IPAddress.Loopback, 80);
					//	options.Listen(IPAddress.Loopback, 449, listenOptions =>
					//	{
					//		listenOptions.UseHttps("localhost.pfx", "duftfinder1");
					//	});
					//}
					)
				.ConfigureServices(services => services.AddAutofac())
				.UseApplicationInsights()
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
					logging.AddConsole();
					logging.AddDebug();
					logging.AddEventSourceLogger();
				})
				.UseStartup<Startup>()
				.UseContentRoot(Directory.GetCurrentDirectory());
		}
	}
}