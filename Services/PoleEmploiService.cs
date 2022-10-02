using bonjourtravail_api.Models;
using bonjourtravail_api.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace bonjourtravail_api.Services
{
    public class PoleEmploiService : IPoleEmploiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly PoleEmploiSettings _poleEmploiSettings;
        private readonly ILogger _logger;

        private static Uri PoleEmploiUri => new UriBuilder(new poleemploi_openapiClient(null).BaseUrl).Uri;

        public PoleEmploiService(IHttpClientFactory clientFactory, IMemoryCache memoryCache, IOptions<PoleEmploiSettings> jobStoreDatabaseSettings, ILogger<PoleEmploiService> logger)
        {
            _httpClient = clientFactory.CreateClient();
            _memoryCache = memoryCache;
            _poleEmploiSettings = jobStoreDatabaseSettings.Value;
            _logger = logger;
        }
        private void AddAuthenticationHeader(AuthenticationResponse authenticationResponse)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authenticationResponse.AccessToken}");
        }
        private async Task<AuthenticationResponse> Authenticate()
        {
            var found = _memoryCache.TryGetValue<AuthenticationResponse>(nameof(AuthenticationResponse), out var value);

            if (found)
            {
                AddAuthenticationHeader(value);

                return value;
            }

            if (string.IsNullOrWhiteSpace(_poleEmploiSettings.UserId)
                || string.IsNullOrWhiteSpace(_poleEmploiSettings.UserSecret))
            {
                throw new UnauthorizedAccessException("Missing credentials in project configuration");
            }

            var body = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", _poleEmploiSettings.UserId},
                {"client_secret", _poleEmploiSettings.UserSecret},
                {"scope", string.Join(' ', new[] { "api_offresdemploiv2", "o2dsoffre"})}
            });
            var uriBuilder = new UriBuilder(_poleEmploiSettings.AuthenticationUrl);
            uriBuilder.Query += "?realm=%2Fpartenaire";

            var res = await _httpClient.PostAsync(uriBuilder.Uri, body);

            if (!res.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException("PoleEmpoi authentication failure");
            }

            var resAsText = await res.Content.ReadAsStringAsync();
            try
            {
                var resAsObject = System.Text.Json.JsonSerializer.Deserialize<AuthenticationResponse>(resAsText);
                _ = _memoryCache.Set(nameof(AuthenticationResponse), resAsObject, TimeSpan.FromSeconds(resAsObject.ExpiresIn));
                AddAuthenticationHeader(resAsObject);
                _logger.LogInformation("PoleEmploi token refreshed");

                return resAsObject;
            }
            catch
            {
                throw new UnauthorizedAccessException("PoleEmpoi authentication failure");
            }
        }

        public async Task<IEnumerable<Job>?> SearchOffers()
        {
            await Authenticate();
            var res = await _httpClient.GetAsync(new UriBuilder(PoleEmploiUri) { Path = PoleEmploiUri.AbsolutePath + "/v2/offres/search" }.Uri);

            if (!res.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException("PoleEmploi API query failure");
            }

            var resAsText = await res.Content.ReadAsStringAsync();
            var resAsObject = Newtonsoft.Json.JsonConvert.DeserializeObject<OfferResponse>(resAsText);
            resAsObject.Results = resAsObject.Results.Take(20);
            return resAsObject?.Results;
        }
    }
}
