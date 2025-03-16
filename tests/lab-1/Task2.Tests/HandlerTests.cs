using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Task2.Tests;

public class HandlerTests
{
    [Fact]
    public async Task SendAsync_ShouldReturnResult_WhenResultIsGivenToHandler()
    {
        byte[] responseRequired = BitConverter.GetBytes(123);
        ILibraryOperationService libraryOperationService = Substitute.For<ILibraryOperationService>();
        Guid resultId = Guid.Empty;
        libraryOperationService.When(x => x.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(callInfo =>
            {
                resultId = callInfo.Arg<Guid>();
            });
        var handlerAsync = new HandlerAsync(libraryOperationService);
        var requestModel = new RequestModel("Method", Array.Empty<byte>());
        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        Task<ResponseModel> task = handlerAsync.SendAsync(requestModel, cancellationToken);
        handlerAsync.HandleOperationResult(resultId, responseRequired);
        ResponseModel responseModel = await task;

        responseModel.Data.Should().BeEquivalentTo(responseRequired);
    }

    [Fact]
    public void SendAsync_ShouldReturnException_WhenExceptionIsGivenToHandler()
    {
        var exception = new Exception();
        ILibraryOperationService libraryOperationService = Substitute.For<ILibraryOperationService>();
        Guid resultId = Guid.Empty;
        libraryOperationService.When(x => x.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(callInfo =>
            {
                resultId = callInfo.Arg<Guid>();
            });
        var handlerAsync = new HandlerAsync(libraryOperationService);
        var requestModel = new RequestModel("Method", Array.Empty<byte>());
        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        Task<ResponseModel> task = handlerAsync.SendAsync(requestModel, cancellationToken);
        handlerAsync.HandleOperationError(resultId, exception);
        Func<Task> func = async () => await task;

        func.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task SendAsync_ShouldReturnTokenException_WhenCancelledTokenIsGiven()
    {
        byte[] responseRequired = BitConverter.GetBytes(123);
        ILibraryOperationService libraryOperationService = Substitute.For<ILibraryOperationService>();
        Guid resultId = Guid.Empty;
        libraryOperationService.When(x => x.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(callInfo =>
            {
                resultId = callInfo.Arg<Guid>();
            });
        var handlerAsync = new HandlerAsync(libraryOperationService);
        var requestModel = new RequestModel("Method", Array.Empty<byte>());
        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        await cancellationTokenSource.CancelAsync();

        Task<ResponseModel> task = handlerAsync.SendAsync(requestModel, cancellationToken);
        handlerAsync.HandleOperationResult(resultId, responseRequired);
        Func<Task> func = async () => await task;

        await func.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task SendAsync_ShouldReturnTokenException_WhenGivenTokenIsCancelled()
    {
        byte[] responseRequired = BitConverter.GetBytes(123);
        ILibraryOperationService libraryOperationService = Substitute.For<ILibraryOperationService>();
        Guid resultId = Guid.Empty;
        libraryOperationService.When(x => x.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(callInfo =>
            {
                resultId = callInfo.Arg<Guid>();
            });
        var handlerAsync = new HandlerAsync(libraryOperationService);
        var requestModel = new RequestModel("Method", Array.Empty<byte>());
        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        Task<ResponseModel> task = handlerAsync.SendAsync(requestModel, cancellationToken);
        await cancellationTokenSource.CancelAsync();
        handlerAsync.HandleOperationResult(resultId, responseRequired);
        Func<Task> func = async () => await task;

        await func.Should().ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task SendAsync_ShouldReturnResult_WhenResultIsGivenInBeginOperation()
    {
        byte[] responseRequired = BitConverter.GetBytes(123);
        ILibraryOperationService libraryOperationService = Substitute.For<ILibraryOperationService>();
        var handlerAsync = new HandlerAsync(libraryOperationService);
        libraryOperationService.When(x => x.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(callInfo =>
            {
                handlerAsync.HandleOperationResult(callInfo.Arg<Guid>(), responseRequired);
            });
        var requestModel = new RequestModel("Method", Array.Empty<byte>());
        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        Task<ResponseModel> task = handlerAsync.SendAsync(requestModel, cancellationToken);
        ResponseModel responseModel = await task;

        responseModel.Data.Should().BeEquivalentTo(responseRequired);
    }

    [Fact]
    public void SendAsync_ShouldReturnException_WhenExceptionIsGivenInBeginOperation()
    {
        var exception = new Exception();
        ILibraryOperationService libraryOperationService = Substitute.For<ILibraryOperationService>();
        var handlerAsync = new HandlerAsync(libraryOperationService);
        libraryOperationService.When(x => x.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(callInfo =>
            {
                handlerAsync.HandleOperationError(callInfo.Arg<Guid>(), exception);
            });
        var requestModel = new RequestModel("Method", Array.Empty<byte>());
        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        Task<ResponseModel> task = handlerAsync.SendAsync(requestModel, cancellationToken);
        Func<Task> func = async () => await task;

        func.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public void SendAsync_ShouldReturnTokenException_WhenTokenIsCanceledInBeginOperation()
    {
        var exception = new Exception();
        ILibraryOperationService libraryOperationService = Substitute.For<ILibraryOperationService>();
        var handlerAsync = new HandlerAsync(libraryOperationService);
        var requestModel = new RequestModel("Method", Array.Empty<byte>());
        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        libraryOperationService.When(x => x.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(_ =>
            {
                cancellationTokenSource.Cancel();
            });

        Task<ResponseModel> task = handlerAsync.SendAsync(requestModel, cancellationToken);
        Func<Task> func = async () => await task;

        func.Should().ThrowAsync<TaskCanceledException>();
    }
}