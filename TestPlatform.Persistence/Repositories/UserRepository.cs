using Microsoft.EntityFrameworkCore;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Persistence.Repositories;

internal sealed class UserRepository(TestPlatformDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken)
        => dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken);

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken)
        => await dbContext.Users
            .OrderBy(user => user.FirstName)
            .ThenBy(user => user.LastName)
            .ToListAsync(cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => dbContext.Users.SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        => dbContext.Users.SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
