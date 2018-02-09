using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using datamakerslib.Startup;
using DotVVM.Framework.Hosting;
using Electro.model.DataContext;
using Electro.model.Repository;
using electroweb.Reports;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace electroweb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
          
            Configuration = configuration;   
        }

         public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddNodeServices();
            services.AddDataProtection();
            services.AddAuthorization();
            services.AddWebEncoders();
            services.AddAutoMapper(); 
            services.AddDotVVM(options =>
            {
                options.AddDefaultTempStorages("Temp");
            });
            services.AddOptions();
            // services.AddDataAccess<MyAppContext>();
              var configurationSection = Configuration.GetSection("ConnectionStrings:DataAccessPostgreSqlProvider").Value;
           // var configureOptions = services.BuildServiceProvider().GetRequiredService<IConfigureOptions<ConnectionStrings>>();
             services.AddDbContext<MyAppContext>(options =>
             {
               // options.UseOpenIddict();
                options.UseNpgsql(
                configurationSection,
                    b => b.MigrationsAssembly("Electroweb")
                );
            });


            //AUTH
            
              services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToReturnUrl = c => DotvvmAuthenticationHelper.ApplyRedirectResponse(c.HttpContext, c.RedirectUri),
                        OnRedirectToAccessDenied = c => DotvvmAuthenticationHelper.ApplyStatusCodeResponse(c.HttpContext, 403),
                        OnRedirectToLogin = c => DotvvmAuthenticationHelper.ApplyRedirectResponse(c.HttpContext, c.RedirectUri),
                        OnRedirectToLogout = c => DotvvmAuthenticationHelper.ApplyRedirectResponse(c.HttpContext, c.RedirectUri)
                    };
                    options.LoginPath = new PathString("/login");
                });

            
             // Add converter to DI
           // services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));   
            //llamar a los repositorios
            services.AddTransient<ITipoCableRepository, TipoCableRepository>(); 
             services.AddTransient<IDetalleTipoCableRepository, DetalleTipoCableRepository>(); 
             services.AddTransient<ITipoNovedadRepository, TipoNovedadRepository>(); 
             services.AddTransient<ITipoEquipoRepository, TipoEquipoRepository>(); 
             services.AddTransient<IDetalleTipoNovedadRepository, DetalleTipoNovedadRepository>(); 
             services.AddTransient<IEmpresaRepository, EmpresaRepository>(); 
             services.AddTransient<IDispositivoRepository, DispositivoRepository>();
             services.AddTransient<IEmpresaRepository, EmpresaRepository>();
             services.AddTransient<ICiudadRepository, CiudadRepository>();   
             services.AddTransient<IDepartamentoRepository, DepartamentoRepository>();
             services.AddTransient<IMaterialRepository, MaterialRepository>();
             services.AddTransient<IEstadoRepository, EstadoRepository>();
             services.AddTransient<IElementoRepository, ElementoRepository>();
             services.AddTransient<IElementoCableRepository, ElementoCableRepository>(); 
            services.AddTransient<IEquipoElementoRepository, EquipoElementoRepository>(); 
              services.AddTransient<ILongitudElementoRepository, LongitudElementoRepository>(); 

             services.AddTransient<INovedadRepository, NovedadRepository>(); 
             services.AddTransient<IFotoRepository, FotoRepository>(); 
                services.AddTransient<IUsuarioRepository, UsuarioRepository>(); 

                 services.AddTransient<IPerdidaRepository, PerdidaRepository>(); 
            //Ciudad Empresa
            services.AddTransient<ICiudad_EmpresaRepository, Ciudad_EmpresaRepository>(); 

             services.AddTransient<ILocalizacionElementoRepository, LocalizacionElementoRepository>(); 

           services.AddScoped<MasterDetailsPdfReport>();
           services.AddDirectoryBrowser();
             //agregar todo las interaces y sus implementacions
               IServiceProvider serviceProvider = services.BuildServiceProvider();
           
              // var context = serviceProvider.GetRequiredService<MyAppContext>();
               var masterdetail= serviceProvider.GetRequiredService<MasterDetailsPdfReport>();    
                 var elemntservice=serviceProvider.GetRequiredService<IElementoRepository>();    
                MasterDetailsPdfReport._IelementosRepository=elemntservice;
              //  DbInitializer.Initialize(context);
              
            //  string wwwroot=Path.Combine("wwwroot");
             // string TheUrl = "http://54.86.105.4/";
               // string remote = GetHtmlPage(TheUrl);
 
        //  MasterDetailsPdfReport.CreateMasterDetailsPdfReport(wwwroot);
             
        }
        static string GetHtmlPage(string strURL)
        {

                String strResult;
                WebResponse objResponse;
                WebRequest objRequest = HttpWebRequest.Create(strURL);
                objResponse = objRequest.GetResponse();
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    strResult = sr.ReadToEnd();
                    sr.Close();
                }
                return strResult;
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

           app.UseAuthentication();

           
            loggerFactory.AddConsole();

            // use DotVVM
            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(env.ContentRootPath);

            // use static files
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(env.WebRootPath)
            });
            app.UseMvcWithDefaultRoute(); 
           // app.UseFileServer(enableDirectoryBrowsing: true);
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
           Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Fotos")),
                RequestPath = new PathString("/Fotos")
            });
          
            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Fotos")),
                RequestPath = new PathString("/Fotos")
            });
            app.UseStaticFiles(new StaticFileOptions()  
            {  
                FileProvider = new PhysicalFileProvider(  
            Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/fonts")),  
                RequestPath = new PathString("/wwwroot/fonts") // accessing outside wwwroot folder contents.  
            }); 

            

            app.UseMvc();
        }
    }
}
