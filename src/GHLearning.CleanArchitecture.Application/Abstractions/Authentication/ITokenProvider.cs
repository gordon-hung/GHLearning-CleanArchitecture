using GHLearning.CleanArchitecture.Core.Users;

namespace GHLearning.CleanArchitecture.Application.Abstractions.Authentication;

public interface ITokenProvider
{
	string Create(UserInfo user);
}
