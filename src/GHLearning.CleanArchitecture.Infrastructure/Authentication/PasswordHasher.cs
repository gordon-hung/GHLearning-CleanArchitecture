using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;

namespace GHLearning.CleanArchitecture.Infrastructure.Authentication;

internal sealed class PasswordHasher : IPasswordHasher
{
	private const int SaltRounds = 10;

	public string Hash(string password)
	{
		// 生成隨機的鹽並哈希密碼
		var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, SaltRounds);
		return hashedPassword;
	}

	public bool Verify(string password, string passwordHash)
	{
		// 驗證密碼是否正確
		return BCrypt.Net.BCrypt.Verify(password, passwordHash);
	}
}
