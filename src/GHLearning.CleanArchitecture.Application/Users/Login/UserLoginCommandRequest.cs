using GHLearning.CleanArchitecture.Application.Abstractions.Messaging;

namespace GHLearning.CleanArchitecture.Application.Users.Login;

public sealed record UserLoginCommandRequest(string Email, string Password) : ICommandRequest<string>;
