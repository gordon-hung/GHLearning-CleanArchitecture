using GHLearning.CleanArchitecture.Core.Users;
using GHLearning.CleanArchitecture.Infrastructure.Entities;
using GHLearning.CleanArchitecture.Infrastructure.Entities.Models;

using Microsoft.EntityFrameworkCore;

namespace GHLearning.CleanArchitecture.Infrastructure.Users;

internal sealed class UserRepository(
	SampleContext context) : IUserRepository
{
	public async Task CreatedAsync(UserCreated source, CancellationToken cancellationToken = default)
	{
		var entity = new User
		{
			Id = source.Id,
			Email = source.Email.ToLower(),
			FirstName = source.FirstName,
			LastName = source.LastName,
			PasswordHash = source.PasswordHash
		};
		await context.Users.AddAsync(entity, cancellationToken).ConfigureAwait(false);
		await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}

	public async Task<UserInfo?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var entity = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken).ConfigureAwait(false);
		return entity is null
			? null
			: new UserInfo(
				Id: entity.Id,
				Email: entity.Email,
				FirstName: entity.FirstName,
				LastName: entity.LastName,
				PasswordHash: entity.PasswordHash);
	}

	public async Task<UserInfo?> GetAsync(string email, CancellationToken cancellationToken = default)
	{
		email = email.ToLower();
		var entity = await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken).ConfigureAwait(false);
		return entity is null
			? null
			: new UserInfo(
				Id: entity.Id,
				Email: entity.Email,
				FirstName: entity.FirstName,
				LastName: entity.LastName,
				PasswordHash: entity.PasswordHash);
	}

	public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
	{
		email = email.ToLower();

		return !await context.Users.AnyAsync(u => u.Email == email, cancellationToken).ConfigureAwait(false);
	}

	public async Task UpdatePasswordHashAsync(UserUpdatePassword source, CancellationToken cancellationToken = default)
	{
		var entity = await context.Users.SingleOrDefaultAsync(u => u.Id == source.Id && u.Email == source.Email, cancellationToken).ConfigureAwait(false)
			?? throw new ArgumentNullException(nameof(source));

		entity.PasswordHash = source.PasswordHash;

		context.Users.Update(entity);
		await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}
}
