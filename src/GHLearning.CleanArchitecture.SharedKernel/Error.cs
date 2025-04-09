namespace GHLearning.CleanArchitecture.SharedKernel;

public record Error
{
	public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Unauthorized);
	public static readonly Error NullValue = new(
		"General.Null",
		"Null value was provided",
		ErrorType.Unauthorized);

	public Error(string code, string description, ErrorType type)
	{
		Code = code;
		Description = description;
		Type = type;
	}

	public string Code { get; }

	public string Description { get; }

	public ErrorType Type { get; }

	public static Error Validation(string code, string description) =>
		new(code, description, ErrorType.Validation);

	public static Error Unauthorized(string code, string description) =>
		new(code, description, ErrorType.Unauthorized);

	public static Error Forbidden(string code, string description) =>
		new(code, description, ErrorType.Forbidden);

	public static Error NotFound(string code, string description) =>
		new(code, description, ErrorType.NotFound);

	public static Error Conflict(string code, string description) =>
		new(code, description, ErrorType.Conflict);

	public static Error InternalServerError(string code, string description) =>
		new(code, description, ErrorType.InternalServerError);

	public static Error NotImplemented(string code, string description) =>
		new(code, description, ErrorType.NotImplemented);
}
