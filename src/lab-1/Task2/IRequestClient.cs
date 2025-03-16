namespace Task2;

public interface IRequestClient
{
    Task<ResponseModel> SendAsync(RequestModel request, CancellationToken cancellationToken);
}