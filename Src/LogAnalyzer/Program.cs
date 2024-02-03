namespace LogAnalyzer
{
    using Core.Services;
    using Core.Services.Interfaces;
    using Git.Services;
    using Git.Services.Interfaces;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Базовый класс проекта.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Стартовая точка проекта.
        /// </summary>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            services.AddEndpointsApiExplorer();
            builder.Services.AddCors();

            builder.Services.AddRazorPages();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers();

            builder.Services.AddTransient<IGitService, GitService>();
            builder.Services.AddTransient<IRepositoryService, RepositoryService>();
            builder.Services.AddTransient<ICoreService, CoreService>();

            var app = builder.Build();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000"));

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.Run();
        }
    }
}