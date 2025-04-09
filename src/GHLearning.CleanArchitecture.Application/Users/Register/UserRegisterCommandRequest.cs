using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

namespace GHLearning.CleanArchitecture.Application.Users.Register;

public sealed record UserRegisterCommandRequest(
	string Email,
	string FirstName,
	string LastName,
	string Password) : ICommandRequest<Guid>;
