using Grpc.Core;

namespace OrderService.Interceptor;

public class ClientInterceptor : Grpc.Core.Interceptors.Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception exception)
        {
            var status = new Status(StatusCode.Internal, $"Error occurred: {exception.Message}");
            var metadata = new Metadata();
            throw new RpcException(status, metadata);
        }
    }
}