using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tgsdfth;

namespace FINAL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=" + AppDomain.CurrentDomain.BaseDirectory.ToString().Replace("bin\\Debug\\netcoreapp3.1\\", "Data\\GROUPPROJECTDATABASE.MDF").ToUpper() + ";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False
                    Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory.ToString().Replace("bin\\Debug\\netcoreapp3.1\\", "Data\\GROUPPROJECTDATABASE.MDF").ToUpper());
                    webBuilder.UseStartup<Startup>();
                });
    }
}
