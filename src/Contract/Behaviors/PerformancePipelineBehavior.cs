namespace Contract;

public class PerformancePipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;

    public PerformancePipelineBehavior(ILogger<TRequest> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();
        var response = await next();
        _timer.Stop();

        var elapsedMillisecond = _timer.ElapsedMilliseconds;
        if (elapsedMillisecond < 5000) return response;

        var requestName = typeof(TRequest).Name;
        _logger.LogWarning("Long Time Running - Request Details : {Name}, ({ElapsedMilliseconds} milliseconds) {@Request}",
            requestName, elapsedMillisecond, request);
        return response;
    }
}
