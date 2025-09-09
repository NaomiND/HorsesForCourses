using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests
{
    public class UserPersistenceTests : DbContextTestBase
    {
        [Fact]
        public async Task User_CanBeSavedAndRetrieved_PropertiesAreCorrect()
        {
            var newUser = User.Create("Ine De Wit", "Ine@example.com", "hashed_password_123");

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var savedUser = await _context.Users.FindAsync(newUser.Id);

            Assert.NotNull(savedUser);
            Assert.Equal("Ine De Wit", savedUser.Name.DisplayName);
            Assert.Equal("Ine@example.com", savedUser.Email.Value);
            Assert.Equal("hashed_password_123", savedUser.PasswordHash);
        }

        [Fact]
        public async Task User_SavingDuplicateEmail_ThrowsDbUpdateException()
        {
            var email = "duplicate@example.com";
            var user1 = User.Create("User Een", email, "pass1");
            var user2 = User.Create("User Twee", email, "pass2");

            _context.Users.Add(user1);
            await _context.SaveChangesAsync(); // First user is saved successfully

            _context.Users.Add(user2);

            await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());      // should throw because email not unique
        }
    }
}