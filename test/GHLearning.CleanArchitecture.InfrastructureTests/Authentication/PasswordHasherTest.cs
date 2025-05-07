using GHLearning.CleanArchitecture.Infrastructure.Authentication;

namespace GHLearning.CleanArchitecture.InfrastructureTests.Authentication;

public class PasswordHasherTest
{
	[Fact]
	public void Hash()
	{
		var sut = new PasswordHasher();

		var actual = sut.Hash("password");

		Assert.NotNull(actual);
	}

	[Fact]
	public void Verify()
	{
		var sut = new PasswordHasher();

		var password = "password";
		var passwordHash = "$2a$10$Y54IPgs4hvjH.4Q6z27LBOMwW9nWedpuvsSTkY98h.8hVDUfmfNka";

		var actual = sut.Verify(password, passwordHash);

		Assert.True(actual);
	}
}
