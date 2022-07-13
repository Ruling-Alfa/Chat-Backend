using Alfa.ChatMS.Data;
using Alfa.ChatMS.Helper;

namespace Alfa.ChatMS
{
    public static class Pipeline
    {
        public static WebApplication UsePipeline(this WebApplication app)
        {
           
            if (!app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
                //app.UseExceptionHandler("/Error");
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("CorsPolicy");
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.EnsureMigrationOfContext<ApplicationDbContext>();
            app.UseEndpoints(endpoints =>
            {                 
                endpoints.MapControllers();
            });
            app.MapHub<ChatHub>("/chat", options =>
            {
                options.Transports =
                    Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
                    Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
            });

            return app;
        }
    }
}