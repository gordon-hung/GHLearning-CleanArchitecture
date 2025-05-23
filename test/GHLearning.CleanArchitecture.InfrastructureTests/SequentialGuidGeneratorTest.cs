using GHLearning.CleanArchitecture.Infrastructure;

namespace GHLearning.CleanArchitecture.InfrastructureTests;
public class SequentialGuidGeneratorTest
{
	[Fact]
	public void NewId()
	{
		var sut = new SequentialGuidGenerator();

		var actual = sut.NewId();

		Assert.NotEqual(Guid.Empty, actual);
	}
}
