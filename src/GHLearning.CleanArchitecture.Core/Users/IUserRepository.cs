using GHLearning.CleanArchitecture.Core.Users;
namespace GHLearning.CleanArchitecture.Core.Users;

public interface IUserRepository
{
	public Task<UserInfo?> GetAsync(Guid userId, CancellationToken cancellationToken = default);

	public Task<UserInfo?> GetAsync(string email, CancellationToken cancellationToken = default);

	public Task CreatedAsync(UserCreated source, CancellationToken cancellationToken = default);

	public Task UpdatePasswordHashAsync(UserUpdatePassword source, CancellationToken cancellationToken = default);

	public Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
}
