// using AutoBogus;
// using MediatR;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Http.HttpResults;
// using NSubstitute;
//
// namespace TDDSample.Tests.UnitTests.Users.Features.GetUsers;
//
// public class GetUsersEndpointTests
// {
// 	[Fact]
// 	public async Task handle_should_return_ok_status_code()
// 	{
// 		var mediator = Substitute.For<IMediator>();
// 		var cancellationToken = CancellationToken.None;
// 		var page = 1;
// 		var pageSize = 10;
// 		var requestParameters = new GetUsersEndpoint.GetUsersRequestParameters(
// 			mediator,
// 			cancellationToken,
// 			page,
// 			pageSize);
// 		var users = new AutoFaker<UserDto>().Generate(20);
// 		var pageResult =
// 			new PagedList<UserDto>
// 			{
// 				PageSize = pageSize,
// 				PageNumber = page,
// 				Results = users
// 			};
//
// 		mediator
// 			.Send(
// 				Arg.Is<TDDSample.Users.GetUsers.GetUsers>(
// 					x => x.PageRequest.Page == page && x.PageRequest.PageSize == pageSize),
// 				Arg.Any<CancellationToken>())
// 			.Returns(pageResult);
//
// 		var result = await GetUsersEndpoint.Handle(requestParameters);
//
// 		result.Should().NotBeNull();
// 		var okResult = result.Result.Should().BeOfType<Ok<PagedList<UserDto>>>().Subject;
// 		okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
// 	}
//
// 	[Fact]
// 	public async Task handle_should_call_mediator_service_with_correct_parameters_once()
// 	{
// 		var mediator = Substitute.For<IMediator>();
// 		var cancellationToken = CancellationToken.None;
// 		var page = 1;
// 		var pageSize = 10;
// 		var requestParameters = new GetUsersEndpoint.GetUsersRequestParameters(
// 			mediator,
// 			cancellationToken,
// 			page,
// 			pageSize);
// 		var users = new AutoFaker<UserDto>().Generate(20);
// 		var pageResult =
// 			new PagedList<UserDto>
// 			{
// 				PageSize = pageSize,
// 				PageNumber = page,
// 				Results = users
// 			};
//
// 		mediator
// 			.Send(
// 				Arg.Is<TDDSample.Users.GetUsers.GetUsers>(
// 					x => x.PageRequest.Page == page && x.PageRequest.PageSize == pageSize),
// 				Arg.Any<CancellationToken>())
// 			.Returns(pageResult);
//
// 		await GetUsersEndpoint.Handle(requestParameters);
//
// 		await mediator
// 			.Received(1)
// 			.Send(
// 				Arg.Is<TDDSample.Users.GetUsers.GetUsers>(
// 					x => x.PageRequest.Page == page && x.PageRequest.PageSize == pageSize),
// 				Arg.Any<CancellationToken>());
// 	}
//
// 	[Fact]
// 	public async Task handle_should_return_correct_users()
// 	{
// 		var mediator = Substitute.For<IMediator>();
// 		var cancellationToken = CancellationToken.None;
// 		var page = 1;
// 		var pageSize = 10;
// 		var requestParameters = new GetUsersEndpoint.GetUsersRequestParameters(
// 			mediator,
// 			cancellationToken,
// 			page,
// 			pageSize);
// 		var users = new AutoFaker<UserDto>().Generate(20);
// 		var pageResult =
// 			new PagedList<UserDto>
// 			{
// 				PageSize = pageSize,
// 				PageNumber = page,
// 				Results = users
// 			};
//
// 		mediator
// 			.Send(
// 				Arg.Is<TDDSample.Users.GetUsers.GetUsers>(
// 					x => x.PageRequest.Page == page && x.PageRequest.PageSize == pageSize),
// 				Arg.Any<CancellationToken>())
// 			.Returns(pageResult);
//
// 		var result = (await GetUsersEndpoint.Handle(requestParameters)).Result;
//
// 		result.Should().BeOfType<Ok<PagedList<UserDto>>>();
// 		var okResult = result.As<Ok<PagedList<UserDto>>>();
// 		okResult.Should().NotBeNull();
//
// 		okResult.Value.Should().NotBeNull();
// 		okResult.Value.PageSize.Should().Be(pageSize);
// 		okResult.Value.PageNumber.Should().Be(page);
// 		okResult.Value.Results.Should().HaveCountGreaterThan(0);
// 		okResult.Value.Should().BeEquivalentTo(pageResult);
// 	}
//
// 	[Fact]
// 	public async Task handle_with_http_response_exception_should_returns_http_problem()
// 	{
// 		// Arrange
// 		var mediator = Substitute.For<IMediator>();
// 		var cancellationToken = CancellationToken.None;
// 		var page = 1;
// 		var pageSize = 10;
// 		var requestParameters = new GetUsersEndpoint.GetUsersRequestParameters(
// 			mediator,
// 			cancellationToken,
// 			page,
// 			pageSize);
//
// 		mediator.Send(Arg.Any<TDDSample.Users.GetUsers.GetUsers>(), cancellationToken)
// 			.Returns(new HttpResponseException(StatusCodes.Status400BadRequest, "NotFound"));
//
// 		// Act
// 		var actualResult = (await GetUsersEndpoint.Handle(requestParameters)).Result;
//
// 		// Assert
// 		var problemHttpResult = actualResult.Should().BeOfType<ProblemHttpResult>().Subject;
// 		problemHttpResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
// 	}
// }
