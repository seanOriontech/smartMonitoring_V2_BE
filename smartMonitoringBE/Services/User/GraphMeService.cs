using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Identity.Web;

namespace smartMonitoringBE.services.user;

public sealed class GraphMeService
{
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly string[] GraphScopes = new[] { "User.Read" };

    public GraphMeService(ITokenAcquisition tokenAcquisition, IHttpClientFactory httpClientFactory)
    {
        _tokenAcquisition = tokenAcquisition;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GraphMeDto> GetMeAsync(CancellationToken ct = default)
    {
        // ✅ This is the key: OBO token for Graph
        var graphToken = await _tokenAcquisition.GetAccessTokenForUserAsync(GraphScopes);

        var http = _httpClientFactory.CreateClient();
        using var req = new HttpRequestMessage(
            HttpMethod.Get,
            "https://graph.microsoft.com/v1.0/me?$select=id,displayName,givenName,surname,mail,userPrincipalName,identities"
        );
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", graphToken);

        using var res = await http.SendAsync(req, ct);
        var body = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"Graph /me failed: {(int)res.StatusCode} {res.ReasonPhrase}. Body: {body}");

        return JsonSerializer.Deserialize<GraphMeDto>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new GraphMeDto();
    }
}

public sealed class GraphMeDto
{
    public string? Id { get; set; }
    public string? DisplayName { get; set; }
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? Mail { get; set; }
    public string? UserPrincipalName { get; set; }
    public object? Identities { get; set; }
}