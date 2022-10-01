
namespace bonjourtravail_api.Models
{
    public class OfferResponse
    {
        [Newtonsoft.Json.JsonProperty("resultats")]
        public IEnumerable<Offre> Results { get; set; }
    }
}
