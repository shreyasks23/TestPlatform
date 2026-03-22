using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

internal sealed class UserRepositoryStub(params IEnumerable<User> seededUsers) : IUserRepository
{
    private readonly List<User> _users = [.. seededUsers];

    public User? AddedUser { get; private set; }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        AddedUser = user;
        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(_users.Any(user => user.Id == userId));

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<User>>(_users.AsReadOnly());

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => Task.FromResult(_users.SingleOrDefault(user => user.Email == email));

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(_users.SingleOrDefault(user => user.Id == userId));

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
