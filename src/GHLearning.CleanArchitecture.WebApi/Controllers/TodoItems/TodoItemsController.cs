using GHLearning.CleanArchitecture.Application.TodoItems.Completed;
using GHLearning.CleanArchitecture.Application.TodoItems.Created;
using GHLearning.CleanArchitecture.Application.TodoItems.Delete;
using GHLearning.CleanArchitecture.Application.TodoItems.Get;
using GHLearning.CleanArchitecture.WebApi.Controllers.TodoItems.ViewModels;
using GHLearning.CleanArchitecture.WebApi.Extensions;
using GHLearning.CleanArchitecture.WebApi.Infrastructure;
using GHLearning.CleanArchitecture.WebApi.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GHLearning.CleanArchitecture.WebApi.Controllers.TodoItems;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TodoItemsController : ControllerBase
{
	[HttpPost]
	public async Task<CustomResultViewModel<TodoItemCreatedResponseViewModel>> TodoItemCreatedAsync(
		[FromServices] ISender sender,
		[FromBody] TodoItemCreatedViewModel source)
	{
		var result = await sender.Send(
			request: new TodoItemCreatedCommandRequest(
				Description: source.Description,
				DueDate: source.DueDate,
				Labels: source.Labels,
				Priority: source.Priority),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result.Match(
			success => CustomResults.Success(success, new TodoItemCreatedResponseViewModel(Id: result.Value)),
			CustomResults.Failure<Guid, TodoItemCreatedResponseViewModel>);
	}

	[HttpGet("{id}")]
	public async Task<CustomResultViewModel<TodoItemGetResponseViewModel>> TodoItemGetAsync(
		[FromServices] ISender sender,
		Guid id)
	{
		var result = await sender.Send(
			request: new TodoItemGetQueryRequest(
				Id: id),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result.Match(
			success => CustomResults.Success(success, new TodoItemGetResponseViewModel(
				Id: result.Value.Id,
				UserId: result.Value.UserId,
				Description: result.Value.Description,
				DueDate: result.Value.DueDate,
				Labels: result.Value.Labels,
				IsCompleted: result.Value.IsCompleted,
				CreatedAt: result.Value.CreatedAt,
				CompletedAt: result.Value.CompletedAt,
				Priority: result.Value.Priority)),
			CustomResults.Failure<TodoItemGetResponse, TodoItemGetResponseViewModel>);
	}

	[HttpDelete("{id}")]
	public async Task<CustomResultViewModel> TodoItemDeleteAsync(
		[FromServices] ISender sender,
		Guid id)
	{
		var result = await sender.Send(
			request: new TodoItemDeleteCommandRequest(
				Id: id),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result.Match(CustomResults.Success, CustomResults.Failure);
	}

	[HttpPatch("{id}/complete")]
	public async Task<CustomResultViewModel> TodoItemCompletedAsync(
		[FromServices] ISender sender,
		Guid id)
	{
		var result = await sender.Send(
			request: new TodoItemCompletedCommandRequest(
				Id: id),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result.Match(CustomResults.Success, CustomResults.Failure);
	}
}
