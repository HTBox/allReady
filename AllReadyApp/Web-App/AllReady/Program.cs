using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AllReady
{
  public class Program
  {
    // Entry point for the application.
    public static void Main(string[] args)
    {
      var host = new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseIISIntegration()
        .UseStartup<Startup>()
        .Build();

      host.Run();
    }
  }
}
