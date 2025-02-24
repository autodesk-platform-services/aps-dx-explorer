using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

public class Program
{
	public static void Main(string[] args)
	{
		CreateHostBuilder(args).Build().Run();
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
					.ConfigureAppConfiguration((hostingContext, config) =>
					{
						var env = hostingContext.HostingEnvironment;

						config.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);

						config.AddEnvironmentVariables();
					})
					.ConfigureWebHostDefaults(webBuilder =>
					{
						webBuilder.UseStartup<Startup>();
					});
}