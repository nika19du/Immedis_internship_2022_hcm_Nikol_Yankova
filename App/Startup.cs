using App.Controllers;
using App.Controllers.Validation;
using App.Helper;
using CloudinaryDotNet;
using Data;
using HCMA.Services.Cloudinaries;
using HCMA.Services.Employees;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App
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
            services.AddDbContext<Context>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<AppAPI>();
            services.AddScoped<IEmployeeService,EmployeeService>(); 
            services.AddTransient<ICloudinaryService, CloudinaryService>();
            services.AddScoped<Validate>();

            var config = new AutoMapper.MapperConfiguration(x =>
              {
                  x.AddProfile(new AppProfile());
              });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            Account cloudinaryCredentials = new Account(
         this.Configuration["Cloudinary:CloudName"],
         this.Configuration["Cloudinary:ApiKey"],
         this.Configuration["Cloudinary:ApiSecret"]);
            Cloudinary cloudinaryUtility = new Cloudinary(cloudinaryCredentials);

            services.AddSingleton(cloudinaryUtility);
             
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel))

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/ErrorPage/{0}") ;
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
