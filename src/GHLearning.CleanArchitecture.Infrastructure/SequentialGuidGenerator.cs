using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Infrastructure;

internal class SequentialGuidGenerator : ISequentialGuidGenerator
{
	public Guid NewId() => SequentialGuid.SequentialGuidGenerator.Instance.NewGuid();
}
