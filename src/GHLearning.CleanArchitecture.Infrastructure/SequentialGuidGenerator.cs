using GHLearning.CleanArchitecture.SharedKernel;

namespace GHLearning.CleanArchitecture.Infrastructure;

internal class SequentialGuidGenerator : ISequentialGuidGenerator
{
	public Task<Guid> NewIdAsync(CancellationToken cancellationToken = default) => Task.FromResult(SequentialGuid.SequentialGuidGenerator.Instance.NewGuid());
}
