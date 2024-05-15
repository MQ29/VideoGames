using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using VideoGames.Infrasctructure;


namespace VideoGames.IntegrationTests
{
    public class VideoGamesTestFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<VideoGamesContext>));
                services.Remove(dbContextDescriptor);
                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbConnection)); 

                services.Remove(dbConnectionDescriptor);

                services
                    .AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<VideoGamesContext>((container, options) =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting").UseInternalServiceProvider(container);
                    });
            });
            builder.UseEnvironment("Development");
        }
    }
}
