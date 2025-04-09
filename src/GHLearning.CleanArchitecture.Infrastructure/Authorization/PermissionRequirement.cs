using Microsoft.AspNetCore.Authorization;

namespace GHLearning.CleanArchitecture.Infrastructure.Authorization;

internal sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
	public string Permission { get; } = permission;
}
