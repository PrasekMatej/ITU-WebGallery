using DotVVM.Contrib;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.Controls.Bootstrap4;
using DotVVM.Framework.ResourceManagement;
using Microsoft.Extensions.DependencyInjection;

namespace WebGallery
{
    public class DotvvmStartup : IDotvvmStartup, IDotvvmServiceConfigurator
    {
        // For more information about this class, visit https://dotvvm.com/docs/tutorials/basics-project-structure
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {
            ConfigureRoutes(config, applicationPath);
            ConfigureControls(config, applicationPath);
            ConfigureResources(config, applicationPath);
        }

        private void ConfigureRoutes(DotvvmConfiguration config, string applicationPath)
        {
            config.RouteTable.Add("Default", "", "Views/Default.dothtml");
            config.RouteTable.Add("PhotoDetail", "PhotoDetail", "Views/PhotoDetail.dothtml");
            config.RouteTable.Add("SignIn", "SignIn", "Views/Authentication/SignIn.dothtml");
            config.RouteTable.Add("Register", "Register", "Views/Authentication/Register.dothtml");
            //config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));    
        }

        private void ConfigureControls(DotvvmConfiguration config, string applicationPath)
        {
            config.AddBootstrap4Configuration();
            config.AddContribFAIconConfiguration();
            config.AddContribGoogleMapConfiguration("AIzaSyCGPSZzWUXLrGJsYQb4QnMH4Yl4rf1b9V0");
        }

        private void ConfigureResources(DotvvmConfiguration config, string applicationPath)
        {
            // register custom resources and adjust paths to the built-in resources
            config.Resources.Register("Styles", new StylesheetResource()
            {
                Location = new UrlResourceLocation("~/styles.css")
            });
            config.Resources.Register("NonAuthenticatedStyles", new StylesheetResource()
            {
                Location = new UrlResourceLocation("~/NonAuthenticated.css")
            });
            config.Resources.Register("PhotoDetailStyles", new StylesheetResource()
            {
                Location = new UrlResourceLocation("~/PhotoDetail.css"),
                Dependencies = new []{ "Styles" }
            });
        }

        public void ConfigureServices(IDotvvmServiceCollection options)
        {
            options.AddDefaultTempStorages("temp");
        }
    }
}
