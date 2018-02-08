using DotVVM.Framework;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Routing;
using electroweb.Controls;

namespace electroweb
{
    public class DotvvmStartup : IDotvvmStartup
    {
        // For more information about this class, visit https://dotvvm.com/docs/tutorials/basics-project-structure
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {
            ConfigureRoutes(config, applicationPath);
            ConfigureControls(config, applicationPath);
            ConfigureResources(config, applicationPath);
            RegisterMarkupControls(config);
            config.AddBusinessPackConfiguration();
        }
        private void RegisterMarkupControls(DotvvmConfiguration config)
        {
            // register markup controls
            /* 
            config.Markup.Controls.Add(new DotvvmControlConfiguration()
            {
                Src = "Controls/UserAvatar.dotcontrol",
                TagPrefix = "cc",
                TagName = "UserAvatar"
            });
            config.Markup.Controls.Add(new DotvvmControlConfiguration()
            {
                Src = "Controls/UserDetailForm.dotcontrol",
                TagPrefix = "cc",
                TagName = "UserDetailForm"
            });

            config.Markup.Controls.Add(new DotvvmControlConfiguration()
            {
                TagPrefix = "cc",
                Namespace = "CheckBook.App.Controls",
                Assembly = "CheckBook.App"
            });
*/
            config.Markup.Controls.Add(new DotvvmControlConfiguration() 
            { 
                TagPrefix = "cc",
                Namespace = "electroweb.Controls",
                Assembly = "electroweb"
            });
        }

        private void ConfigureRoutes(DotvvmConfiguration config, string applicationPath)
        {


             config.RouteTable.Add("Login", "", "Views/Login_User/Login.dothtml");


             config.RouteTable.Add("Default", "Default", "Views/Default.dothtml");
          

            
            config.RouteTable.Add("Elementos", "Elementos", "Views/Elementos.dothtml");
            config.RouteTable.Add("Empresa", "Empresas", "Views/Empresa.dothtml");
       
            config.RouteTable.Add("About", "about", "Views/About.dothtml");
            config.RouteTable.Add("SampleA", "SampleA", "Views/SampleA.dothtml");
            config.RouteTable.Add("SampleB", "SampleB/{Id}", "Views/SampleB.dothtml");
            config.RouteTable.Add("ReportDetalle", "ReportDetalle", "Views/ReportDetalle.dothtml");

            config.RouteTable.Add("ReportDetalleImages", "ReportDetalleImages", "Views/ReportDetalleImages.dothtml");
            config.RouteTable.Add("ReportUsuario", "ReportUsuario", "Views/ReportUsuario/ReportUsuarios.dothtml");
            config.RouteTable.Add("ReportNovedades", "ReportNovedades", "Views/Novedades/NovedadesReport.dothtml");

              config.RouteTable.Add("Administrador", "Administrador", "Views/Administrador/Administrator_Info.dothtml");
           
            
            //Uncomment the following line to auto-register all dothtml files in the Views folder
            //config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));    
            //auto-discover all missing parameterless routes
            //config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));
        }

        private void  ConfigureControls(DotvvmConfiguration config, string applicationPath)
        {
            // register code-only controls and markup controls
         
             config.Markup.AddCodeControls("cc", typeof(ConfirmPostBackHandlerCustom));
            
        }

        private void ConfigureResources(DotvvmConfiguration config, string applicationPath)
        {
            // register custom resources and adjust paths to the built-in resources
         // register custom scripts
         // register custom resources and adjust paths to the built-in resources
            config.Resources.Register("cards", new StylesheetResource(new FileResourceLocation("~/wwwroot/Stylesheets/style_cards.css")));
            config.Resources.Register("style", new StylesheetResource(new FileResourceLocation("~/wwwroot/Stylesheets/style.css")));
            config.Resources.Register("bootstrrap", new StylesheetResource(new FileResourceLocation("~/wwwroot/Stylesheets/bootstrap.min.css")));
           
            config.Resources.Register("jquery_min", new ScriptResource(new FileResourceLocation("~/wwwroot/_vendor/jquery/dist/jquery.min.js")));
            config.Resources.Register("bootstrap_min", new ScriptResource(new FileResourceLocation("~/wwwroot/_vendor/bootstrap/dist/js/bootstrap.min.js")));
            config.Resources.Register("settings", new ScriptResource(new FileResourceLocation("~/wwwroot/scripts/settings.js")));

            /* 
            config.Resources.Register("myscript", new ScriptResource()
            {
                Location = new LocalFileResourceLocation("~/wwwroot/Scripts/myscript.js")
            });*/
         

            config.Resources.Register("autoHideAlert", new ScriptResource()
            {
                Location = new FileResourceLocation("Scripts/autoHideAlert.js"),
                Dependencies = new[] { "jquery" }
            });
            
            
            /* 
            config.Resources.Register("example", new ScriptResource()
            {
                Location = new FileResourceLocation("Scripts/example.js"),
                Dependencies = new[] { "jquery" }
            });*/

         

        

            config.Resources.Register("preserveTextBoxFocus", new ScriptResource()
            {
                Location = new FileResourceLocation("Scripts/preserveTextBoxFocus.js"),
                Dependencies = new[] { "dotvvm", "jquery" }
            });
            config.Resources.Register("ExpressionTextBox", new ScriptResource()
            {
                Location = new FileResourceLocation("Scripts/ExpressionTextBox.js"),
                Dependencies = new [] { "dotvvm", "jquery" }
            });

            // Note that the 'jquery' resource is registered in DotVVM and points to official jQuery CDN.
            // We have jQuery in our application, so we have to change its URL
            ((ScriptResource)config.Resources.FindResource("jquery"))
                .Location = new FileResourceLocation("Scripts/jquery-2.1.3.min.js");

            // register bootstrap
            config.Resources.Register("bootstrap", new ScriptResource()
            {
                Location = new FileResourceLocation("Scripts/bootstrap.min.js"),
                Dependencies = new[] { "jquery" }
            });
        }
    }
}
