using bonjourtravail_api.Models;

namespace bonjourtravail_api.Services
{
    public interface IPoleEmploiService
    {
        Task<IEnumerable<Offre>?> SearchOffers();
    }
}