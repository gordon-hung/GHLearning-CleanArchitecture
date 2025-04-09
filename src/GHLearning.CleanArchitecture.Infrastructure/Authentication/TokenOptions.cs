namespace GHLearning.CleanArchitecture.Infrastructure.Authentication;

public record TokenOptions
{
	public required string Secret { get; init; }
	public required string Issuer { get; init; }
	public required string Audience { get; init; }
	public required int ExpirationInMinutes { get; init; }
}
