using ExtraLife.Entities;
using ExtraLife.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExtraLife
{
    public class ExtraLifeApiClient : IExtraLifeApiClient
    {
        private readonly HttpClient _client;

        public ExtraLifeApiClient(HttpClient client)
        {
            _client = client;
        }

        public Task<IEnumerable<Participant>> GetParticipantsAsync(int page, int limit) =>
            ApiGet<IEnumerable<Participant>>($"participants{GetPaginationQuery(page, limit)}");

        public Task<IEnumerable<Participant>> GetAllParticipantsAsync() =>
            ApiGetAll<Participant>($"participants");

        public Task<Participant> GetParticipantAsync(int participantId) =>
            ApiGet<Participant>($"participants/{participantId}");

        public Task<IEnumerable<Donation>> GetParticipantDonationsAsync(int participantId, int page, int limit) =>
            ApiGet<IEnumerable<Donation>>($"participants/{participantId}/donations{GetPaginationQuery(page, limit)}");

        public Task<IEnumerable<Donation>> GetAllParticipantDonationsAsync(int participantId) =>
            ApiGetAll<Donation>($"participants/{participantId}/donations");

        public Task<IEnumerable<Donor>> GetParticipantDonorsAsync(int participantId, int page, int limit) =>
            ApiGet<IEnumerable<Donor>>($"participants/{participantId}/donors{GetPaginationQuery(page, limit)}");

        public Task<IEnumerable<Donor>> GetAllParticipantDonorsAsync(int participantId) =>
            ApiGetAll<Donor>($"participants/{participantId}/donors");

        public Task<IEnumerable<Activity>> GetAllParticipantActivitiesAsync(int participantId) =>
            ApiGetAll<Activity>($"participants/{participantId}/activity");

        private static string GetPaginationQuery(int page, int limit) => $"?offset={(page == 1 ? 1 : limit * (page - 1))}&limit={limit}";

        private async Task<T> ApiGet<T>(string uri)
        {
            var (item, _) = await ApiGetInternal<T>(uri);

            return item;
        }

        private async Task<IEnumerable<T>> ApiGetAll<T>(string uri)
        {
            var output = new List<T>();

            do
            {
                var (items, linkHeader) = await ApiGetInternal<IEnumerable<T>>(uri);
                output.AddRange(items ?? Enumerable.Empty<T>());
                uri = linkHeader?.NextLink;
            } while (!string.IsNullOrEmpty(uri));

            return output;
        }

        private async Task<(T, LinkHeader)> ApiGetInternal<T>(string uri)
        {
            var response = await _client.GetAsync(uri);

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiResponseException(response.ReasonPhrase) { HttpResponseCode = response.StatusCode };
            }

            var jsonContent = await response.Content.ReadAsStringAsync();

            var item = JsonSerializer.Deserialize<T>(jsonContent);

            var linkHeader = response.Headers.TryGetValues("link", out var link) ? LinkHeader.ParseLinkHeader(link.First()) : default;

            return (item, linkHeader);
        }
    }
}