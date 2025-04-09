using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

namespace GHLearning.CleanArchitecture.Application.Users.GetById;

public record UserGetByIdQueryRequest(
	Guid Id) : IQueryRequest<UserGetByIdQueryResponse>;
