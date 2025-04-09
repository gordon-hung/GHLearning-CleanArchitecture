namespace GHLearning.CleanArchitecture.Infrastructure.Authorization;

internal sealed class PermissionProvider
{
	public static Task<HashSet<string>> GetForUserIdAsync(Guid userId)
	{
		// TODO: Here you'll implement your logic to fetch permissions.
		HashSet<string> permissionsSet = [];

		return Task.FromResult(permissionsSet);
	}
}
