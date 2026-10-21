using Cart.API.Logging;

namespace Cart.API.Clients;

public class CorrelationIdHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpContext? contexto = _httpContextAccessor.HttpContext;
        if (contexto != null)
        {
            string? correlationId = contexto.Items[CorrelationIdMiddleware.HeaderName] as string;
            if (!string.IsNullOrEmpty(correlationId))
            {
                request.Headers.Remove(CorrelationIdMiddleware.HeaderName);
                request.Headers.Add(CorrelationIdMiddleware.HeaderName, correlationId);
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}