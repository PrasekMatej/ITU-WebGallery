﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Identity;
using DotVVM.Framework.Routing;
using WebGallery.BL.DAL;
using WebGallery.BL.Services;

namespace WebGallery
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddAuthorization();
            services.AddWebEncoders();
            services.AddSingleton<Func<GalleryDbContext>>(() =>
            {
                var ob = new DbContextOptionsBuilder<GalleryDbContext>();
                var con = Configuration.GetConnectionString("DefaultConnection");
                ob.UseSqlServer(con);
                return new GalleryDbContext(ob.Options);
            });
            services.AddTransient(typeof(UserService));
            services.AddTransient(typeof(DirectoryService));
            services.AddTransient(typeof(PhotoService));
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<GalleryDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                });
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<GalleryDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(o => { o.LoginPath = new PathString("/SignIn"); });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/SignIn";
                });
            services.AddDotVVM<DotvvmStartup>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseAuthentication();
            // use DotVVM
            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(env.ContentRootPath);
            dotvvmConfiguration.AssertConfigurationIsValid();

            // use static files
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(env.WebRootPath)
            });

        }
    }
}
