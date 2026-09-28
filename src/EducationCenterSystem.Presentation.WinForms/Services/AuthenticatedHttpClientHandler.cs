using System.Net.Http.Headers;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

namespace EducationCenterSystem.Presentation.WinForms.Services;

public sealed class AuthenticatedHttpClientHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    public AuthenticatedHttpClientHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_tokenProvider.Token) && request.Headers.Authorization is null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.Token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
