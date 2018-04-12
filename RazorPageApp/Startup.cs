using System.IO;
using System.Reflection;
using DataLayer.EfCode;
using GenericServices.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ServiceLayer.HomeController.Dtos;

namespace RazorPageApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // thanks to https://exceptionnotfound.net/setting-a-custom-default-page-in-asp-net-core-razor-pages/
            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/Home/Index", "");
            });

            //--------------------------------------------------------------------
            //var connection = Configuration.GetConnectionString("DefaultConnection");
            //Swapped over to sqlite in-memory database
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            connection.Open();  //see https://github.com/aspnet/EntityFramework/issues/6968
            services.AddDbContext<EfCoreContext>(options => options.UseSqlite(connection));
            //--------------------------------------------------------------------

            services.GenericServicesSimpleSetup<EfCoreContext>(Assembly.GetAssembly(typeof(BookListDto)));

            //This is the version you would use if you were registering multiple DbContext
            //services.ConfigureGenericServicesEntities(typeof(BookDbContext), typeof(OrderDbContext))
            //    .ScanAssemblesForDtos(Assembly.GetAssembly(typeof(BookListDto)))
            //    .RegisterGenericServices();

            //--------------------------------------------------------------------------------------------
            //I removed the code below, as this was only needed for the hand-coded versions of the services
            ////Now I use AutoFac to do some of the more complex registering of services
            //var containerBuilder = new ContainerBuilder();

            ////Now I use the ServiceLayer AutoFac module that registers all the other DI items, such as my biz logic
            //containerBuilder.RegisterModule(new ServiceLayerModule());

            //containerBuilder.Populate(services);
            //var container = containerBuilder.Build();
            //return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            // thanks to https://wildermuth.com/2017/11/19/ASP-NET-Core-2-0-and-the-End-of-Bower for this trick
            if (env.IsDevelopment())
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), @"node_modules")),
                    RequestPath = new PathString("/vendor")
                });
            }

            app.UseMvc();
        }
    }
}
