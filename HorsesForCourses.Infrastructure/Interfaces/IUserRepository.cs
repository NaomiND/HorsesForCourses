using HorsesForCourses.Core;

namespace HorsesForCourses.Infrastructure;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}