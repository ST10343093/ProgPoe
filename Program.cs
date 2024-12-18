using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using ProgPoe.Data;

namespace ProgPoe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //Adding DB Context builder services with options
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                       options.UseSqlServer(builder.Configuration.GetConnectionString("Prog6212DEV")));

            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
            //Added service for Authorization for Role based Access
            builder.Services.AddDefaultIdentity<IdentityUser>().AddDefaultTokenProviders()
                           .AddRoles<IdentityRole>()
                           .AddEntityFrameworkStores<ApplicationDbContext>();

            // Add FluentValidation

            builder.Services.AddControllersWithViews()

                .AddFluentValidation(config =>

                {

                    config.RegisterValidatorsFromAssemblyContaining<Program>(); // Automatically register all validators in the project

                });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
