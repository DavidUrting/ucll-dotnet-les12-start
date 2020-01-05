using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorks.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdventureWorks.Web
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
            services.AddControllersWithViews();
            
            // services.AddTransient<ICustomerManager, DummyCustomerManager>();
            services.AddTransient<ICustomerManager, CustomerManager>(s =>
                new CustomerManager(Configuration.GetConnectionString("AdventureWorksWebContextConnection")));
            services.AddTransient<IProductManager, ProductManager>(s =>
                new ProductManager(Configuration.GetConnectionString("AdventureWorksWebContextConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                      name: "areas",
                      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                    );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //Alternatief voor oefening 8.2: convention-based routing
                //endpoints.MapControllerRoute(
                //    name: "zoeken",
                //    pattern: "zoeken",
                //    defaults: new { controller = "Customer", action = "Search" });
            });

            CreateRoles(serviceProvider).Wait();
        }

        readonly string[] ROLES = new string[] { "Admin", "Customer", "Employee", "Sales", "SalesMgmt" };
        // Gebaseerd op https://dotnetdetail.net/role-based-authorization-in-asp-net-core-3-0/
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            IdentityResult roleResult;
            //here in this line we are adding the Roles
            foreach (string role in ROLES) 
            {
                var roleCheck = await RoleManager.RoleExistsAsync(role);
                if (!roleCheck)
                {
                    //here in this line we are creating admin role and seed it to the database
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(role));
                }
            }

            //here we are assigning the Admin role to the User that we have registered above 
            //Now, we are assinging admin role to this user("Ali@gmail.com"). When will we run this project then it will
            //be assigned to that user.
            IdentityUser user = await UserManager.FindByEmailAsync("adw-admin@ucll.be");
            if (user != null)
            {
                foreach (string role in ROLES)
                {
                    await UserManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
