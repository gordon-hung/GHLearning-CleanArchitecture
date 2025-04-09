using System.Security.Claims;
using System.Text;
using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Core.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GHLearning.CleanArchitecture.Infrastructure.Authentication;

internal sealed class TokenProvider(IOptions<TokenOptions> options) : ITokenProvider
{
	public string Create(UserInfo user)
	{
		string secretKey = options.Value.Secret;
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(
			[
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email)
			]),
			Expires = DateTime.UtcNow.AddMinutes(options.Value.ExpirationInMinutes),
			SigningCredentials = credentials,
			Issuer = options.Value.Issuer,
			Audience = options.Value.Audience
		};

		var handler = new JsonWebTokenHandler();

		string token = handler.CreateToken(tokenDescriptor);

		return token;
	}
}
