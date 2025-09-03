using System.Net;
using Microsoft.Extensions.DependencyInjection;
using HorsesForCourses.Core;
using HorsesForCourses.Application;

namespace HorsesForCourses.Tests.Integration
{
    public class CoachesControllerIntegrationTests : IClassFixture<CustomWebAppFactory>
    {
        private readonly HttpClient _client;

        public CoachesControllerIntegrationTests(CustomWebAppFactory factory)
        {
            _client = factory.CreateClient();
            SeedTestData(factory);
        }

        private void SeedTestData(CustomWebAppFactory factory)
        {
            using var scope = factory.Services.CreateScope();
            var services = scope.ServiceProvider;

            // Je echte DbContext gebruiken (pas aan indien anders)
            var context = services.GetRequiredService<AppDbContext>();

            // Database schoonmaken (alleen bij InMemory of SQLite)
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Testgegevens toevoegen
            var coach = new Coach(FullName.From("Test Coach"), EmailAddress.Create("test@coach.com"));
            context.Add(coach);
            context.SaveChanges();
        }

        [Fact]
        public async Task Get_Index_ReturnsOkAndContainsCoach()
        {
            var response = await _client.GetAsync("/Coaches?page=1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Test Coach", content);
        }
    }
}
