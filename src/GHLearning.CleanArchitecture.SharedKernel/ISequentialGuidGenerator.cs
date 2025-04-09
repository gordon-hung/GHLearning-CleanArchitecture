namespace GHLearning.CleanArchitecture.SharedKernel;

public interface ISequentialGuidGenerator
{
	Task<Guid> NewIdAsync(CancellationToken cancellationToken = default);
}
