namespace Contract;

public class TrackingPipelineBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;

    public TrackingPipelineBehavior(ILogger<TRequest> logger)
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
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Request Details: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}", 
            requestName, elapsedMillisecond ,request);
        return response;
    }
}
