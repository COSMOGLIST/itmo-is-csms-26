using System.Collections.Concurrent;

namespace Task2;

public class HandlerAsync(ILibraryOperationService libraryOperationService) : IRequestClient, ILibraryOperationHandler
{
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<ResponseModel>> _responseLibrary = new();

    public async Task<ResponseModel> SendAsync(RequestModel request, CancellationToken cancellationToken)
    {
        var result = new TaskCompletionSource<ResponseModel>(cancellationToken);
        var requestId = Guid.NewGuid();
        await using (cancellationToken.Register(() =>
                     {
                         _responseLibrary.TryRemove(requestId, out TaskCompletionSource<ResponseModel>? value);
                         result.TrySetCanceled(cancellationToken);
                     }))
        {
            _responseLibrary.TryAdd(requestId, result);
            libraryOperationService.BeginOperation(requestId, request, cancellationToken);
            try
            {
                return await result.Task;
            }
            finally
            {
                _responseLibrary.TryRemove(requestId, out TaskCompletionSource<ResponseModel>? value);
            }
        }
    }

    public void HandleOperationResult(Guid requestId, byte[] data)
    {
        if (_responseLibrary.TryGetValue(requestId, out TaskCompletionSource<ResponseModel>? value))
        {
            value.SetResult(new ResponseModel(data));
        }
    }

    public void HandleOperationError(Guid requestId, Exception exception)
    {
        if (_responseLibrary.TryGetValue(requestId, out TaskCompletionSource<ResponseModel>? value))
        {
            value.SetException(exception);
        }
    }
}