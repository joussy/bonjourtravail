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
        private readonly PoleEmploiSettings _jobStoreDatabaseSettings;

        public PoleEmploiService(IHttpClientFactory clientFactory, IMemoryCache memoryCache, IOptions<PoleEmploiSettings> jobStoreDatabaseSettings)
        {
            _httpClient = clientFactory.CreateClient();
            _memoryCache = memoryCache;
            _jobStoreDatabaseSettings = jobStoreDatabaseSettings.Value;
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

            if (string.IsNullOrWhiteSpace(_jobStoreDatabaseSettings.UserId)
                || string.IsNullOrWhiteSpace(_jobStoreDatabaseSettings.UserSecret))
            {
                throw new UnauthorizedAccessException("Missing credentials in project configuration");
            }

            var body = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", _jobStoreDatabaseSettings.UserId},
                {"client_secret", _jobStoreDatabaseSettings.UserSecret},
                {"scope", string.Join(' ', new[] {"api_offresdemploiv2", "o2dsoffre"})}
            });

            var res = await _httpClient.PostAsync("https://entreprise.pole-emploi.fr/connexion/oauth2/access_token?realm=%2Fpartenaire", body);

            if (!res.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException("PoleEmpoi authentication failure");
            }

            var resAsText = await res.Content.ReadAsStringAsync();
            try
            {
                var resAsObject = System.Text.Json.JsonSerializer.Deserialize<AuthenticationResponse>(resAsText);
                _ = _memoryCache.Set(resAsObject, nameof(AuthenticationResponse), TimeSpan.FromSeconds(resAsObject.ExpiresIn));
                AddAuthenticationHeader(resAsObject);

                return resAsObject;
            }
            catch
            {
                throw new UnauthorizedAccessException("PoleEmpoi authentication failure");
            }
        }

        public async Task<IEnumerable<bonjourtravail_api.Offre>?> SearchOffers()
        {
            await Authenticate();
            var res = await _httpClient.GetAsync("https://api.emploi-store.fr/partenaire/offresdemploi/v2/offres/search");
            var resAsText = await res.Content.ReadAsStringAsync();
            var resAsObject = Newtonsoft.Json.JsonConvert.DeserializeObject<OfferResponse>(resAsText);
            resAsObject.Results = resAsObject.Results.Take(20);
            return resAsObject?.Results;
        }
    }
}
