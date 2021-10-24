using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ExtraLife.Entities;

namespace ExtraLife
{
    public class ExtraLifeApiClient : IExtraLifeApiClient
    {
        private const string StandardBaseUrl = "https://www.extra-life.org/api";

        private string BaseUrl { get; }

        public ExtraLifeApiClient() : this(StandardBaseUrl) { }

        public ExtraLifeApiClient(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public Task<IEnumerable<Participant>> GetParticipantsAsync(int page, int limit) =>
            ApiGet<IEnumerable<Participant>>($"{BaseUrl}/participants{GetPaginationQuery(page, limit)}");

        public Task<Participant> GetParticipantAsync(int participantId) =>
            ApiGet<Participant>($"{BaseUrl}/participants/{participantId}");

        public Task<IEnumerable<Donation>> GetParticipantDonationsAsync(int participantId, int page, int limit) =>
            ApiGet<IEnumerable<Donation>>($"{BaseUrl}/participants/{participantId}/donations{GetPaginationQuery(page, limit)}");

        public Task<IEnumerable<Donor>> GetParticipantDonorsAsync(int participantId, int page, int limit) =>
            ApiGet<IEnumerable<Donor>>($"{BaseUrl}/participants/{participantId}/donors{GetPaginationQuery(page, limit)}");

        public Task<Donor> GetDonorAsync(string donorId) => ApiGet<Donor>($"{BaseUrl}/donors/{donorId}");

        public Task<IEnumerable<Activity>> GetParticipantActivitiesAsync(int participantId) =>
            ApiGet<IEnumerable<Activity>>($"{BaseUrl}/participants/{participantId}/activity");

        private static string GetPaginationQuery(int page, int limit) => $"?offset={(page == 1 ? 1 : limit * (page - 1))}&limit={limit}";

        private static async Task<T> ApiGet<T>(string uri)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(uri);

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiResponseException(response.ReasonPhrase) { HttpResponseCode = response.StatusCode };
            }

            return await response.Content.ReadAsAsync<T>();
        }
    }
}