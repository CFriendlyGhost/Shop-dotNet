using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shop.DataContext;
using Shop.Options;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using System;
using Shop.Services;
using Azure.Identity;
using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;


namespace Shop
{
    public class Startup
    {
        public AzureOptions AzureOptions { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IImageService, ImageService>();
            services.AddControllersWithViews();

            var keyVaultValue = Configuration["KeyVault"];
            var client = new SecretClient(new Uri(keyVaultValue), new DefaultAzureCredential());
            KeyVaultSecret secret = client.GetSecret("ConnectionStrings--ShopCS");

            string connectionDbString = secret.Value;

            services.AddDbContext<ShopDbContext>(options =>
                options.UseSqlServer(connectionDbString));

            services.AddSingleton(x =>
            {
                KeyVaultSecret secret = client.GetSecret("BlobStorage--connection");
                var blobContainerClient = new BlobContainerClient(secret.Value, "shopimages");
                return blobContainerClient;
            });


            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ShopDbContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            IdentityDataInitilizer.SeedData(userManager, roleManager);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
