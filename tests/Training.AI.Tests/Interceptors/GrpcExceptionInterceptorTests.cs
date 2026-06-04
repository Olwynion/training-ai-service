using Grpc.Core;
using Grpc.Core.Interceptors;
using Moq;
using Training.AI.Interceptors;

namespace Training.AI.Tests.Interceptors;

public class GrpcExceptionInterceptorTests
{
    private readonly GrpcExceptionInterceptor _interceptor = new();

    [Fact]
    public async Task Handle_WhenInvalidOperationException_ShouldReturnInvalidArgument()
    {
        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _interceptor.UnaryServerHandler<object, object>(
                new object(),
                Mock.Of<ServerCallContext>(),
                (_, _) => throw new InvalidOperationException("test error")));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Equal("test error", ex.Status.Detail);
    }

    [Fact]
    public async Task Handle_WhenUnauthorizedAccessException_ShouldReturnUnauthenticated()
    {
        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _interceptor.UnaryServerHandler<object, object>(
                new object(),
                Mock.Of<ServerCallContext>(),
                (_, _) => throw new UnauthorizedAccessException("unauthorized")));

        Assert.Equal(StatusCode.Unauthenticated, ex.StatusCode);
        Assert.Equal("unauthorized", ex.Status.Detail);
    }

    [Fact]
    public async Task Handle_WhenRpcException_ShouldRethrow()
    {
        var original = new RpcException(new Status(StatusCode.NotFound, "not found"));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _interceptor.UnaryServerHandler<object, object>(
                new object(),
                Mock.Of<ServerCallContext>(),
                (_, _) => throw original));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task Handle_WhenSuccess_ShouldReturnResult()
    {
        Task<object> Success(object req, ServerCallContext ctx) => Task.FromResult<object>("success");

        var result = await _interceptor.UnaryServerHandler<object, object>(
            new object(),
            Mock.Of<ServerCallContext>(),
            Success);

        Assert.Equal("success", result);
    }
}
