using HorsesForCourses.Core;
using HorsesForCourses.Infrastructure;
using HorsesForCourses.Application;
using System.Linq;

namespace HorsesForCourses.Tests
{
    public class PagingAndProjectionTests : DbContextTestBase
    {
        [Fact]
        public async Task GetAllPagedAsync_ForCoaches_ReturnsCorrectPageAndProjectsToDTO()
        {
            for (int i = 1; i <= 25; i++)                                       //db met 25 coaches
            {
                var name = FullName.From($"Coach {i:D2}");
                var email = EmailAddress.From($"coach{i:D2}@test.com");
                _context.Coaches.Add(new Coach(name, email));
            }
            await _context.SaveChangesAsync();

            var repository = new EfCoachRepository(_context);
            var request = new PageRequest(PageNumber: 2, PageSize: 10);

            var pagedResult = await repository.GetAllPagedAsync(request);       //vraag p2 op

            Assert.NotNull(pagedResult);
            Assert.Equal(25, pagedResult.TotalCount);
            Assert.Equal(10, pagedResult.Items.Count);
            Assert.Equal(2, pagedResult.PageNumber);
            Assert.Equal(3, pagedResult.TotalPages);

            var firstCoachOnPage = pagedResult.Items.First();                   // Controleer of de projectie en sortering klopt.
            Assert.Equal("Coach 11", firstCoachOnPage.Name);                    // De eerste coach op pagina 2 moet "Coach 11" zijn.
            Assert.Equal("coach11@test.com", firstCoachOnPage.Email);
        }
    }
}

