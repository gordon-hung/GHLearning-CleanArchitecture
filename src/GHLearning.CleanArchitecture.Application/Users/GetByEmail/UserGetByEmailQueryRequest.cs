using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

namespace GHLearning.CleanArchitecture.Application.Users.GetByEmail;

public record UserGetByEmailQueryRequest(
	string Email) : IQueryRequest<UserGetByEmailQueryResponse>;
