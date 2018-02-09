using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;

namespace electroweb
{
    public class Program
    {
        /* 
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(options => 
                { 
                    options.Limits.MinResponseDataRate = null;
                    options.Limits.MaxRequestBodySize=null;
                   
                })
                //.UseWebRoot("http://54.86.105.4/wwwroot")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5001/")
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                IHostingEnvironment env = builderContext.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .Build();

            host.Run();
        }*/

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(options => 
                { 
                    options.Limits.MinResponseDataRate = null;
                    options.Limits.MaxRequestBodySize=null;
                   options.Limits.MaxConcurrentConnections = 100;
    options.Limits.MaxConcurrentUpgradedConnections = 100;
   
    options.Limits.MinRequestBodyDataRate =
        new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
    options.Limits.MinResponseDataRate =
        new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
    //options.Listen(IPAddress.Loopback, 5000);
                })
                //.UseWebRoot("http://54.86.105.4/wwwroot")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5001/")
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                IHostingEnvironment env = builderContext.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .Build();

            host.Run();
        }
    }
}