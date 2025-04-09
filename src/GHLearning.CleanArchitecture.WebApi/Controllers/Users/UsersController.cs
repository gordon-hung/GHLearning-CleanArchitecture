using GHLearning.CleanArchitecture.Application.Users.GetByEmail;

using GHLearning.CleanArchitecture.Application.Users.GetById;
using GHLearning.CleanArchitecture.Application.Users.Login;

using GHLearning.CleanArchitecture.Application.Users.Register;
using GHLearning.CleanArchitecture.WebApi.Controllers.Users.ViewModels;
using GHLearning.CleanArchitecture.WebApi.Extensions;
using GHLearning.CleanArchitecture.WebApi.Infrastructure;
using GHLearning.CleanArchitecture.WebApi.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GHLearning.CleanArchitecture.WebApi.Controllers.Users;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<CustomResultViewModel<UserLoginResponseViewModel>> UserLoginAsync(
		[FromServices] ISender sender,
		[FromBody] UserLoginViewModel source)
	{
		var result = await sender.Send(
			request: new UserLoginCommandRequest(
				source.Email,
				source.Password),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result.Match(
			success => CustomResults.Success(success, new UserLoginResponseViewModel(Token: result.Value)),
			CustomResults.Failure<string, UserLoginResponseViewModel>);
	}

	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<CustomResultViewModel<UserRegisterResponseViewModel>> UserRegisterAsync(
		[FromServices] ISender sender,
		[FromBody] UserRegisterViewModel source)
	{
		var result = await sender.Send(
			request: new UserRegisterCommandRequest(
				source.Email,
				source.FirstName,
				source.LastName,
				source.Password),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result.Match(
			success => CustomResults.Success(success, new UserRegisterResponseViewModel(Id: result.Value)),
			CustomResults.Failure<Guid, UserRegisterResponseViewModel>);
	}

	[HttpGet("{id:guid}")]
	public async Task<CustomResultViewModel<UserGetResponseViewModel>> UserGetByIdAsync(
		[FromServices] ISender sender,
		Guid id)
	{
		var result = await sender.Send(
			request: new UserGetByIdQueryRequest(id),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result.Match(
			success => CustomResults.Success(success, new UserGetResponseViewModel(
				Id: result.Value.Id,
				Email: result.Value.Email,
				FirstName: result.Value.FirstName,
				LastName: result.Value.LastName)),
			CustomResults.Failure<UserGetByIdQueryResponse, UserGetResponseViewModel>);
	}

	[HttpGet("{email}")]
	public async Task<CustomResultViewModel<UserGetResponseViewModel>> UserGetByEmailAsync(
		[FromServices] ISender sender,
		string email)
	{
		var result = await sender.Send(
			request: new UserGetByEmailQueryRequest(email),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result.Match(
			success => CustomResults.Success(success, new UserGetResponseViewModel(
				Id: result.Value.Id,
				Email: result.Value.Email,
				FirstName: result.Value.FirstName,
				LastName: result.Value.LastName)),
			CustomResults.Failure<UserGetByEmailQueryResponse, UserGetResponseViewModel>);
	}
}
