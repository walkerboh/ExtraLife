using System;
using System.Linq;
using System.Threading.Tasks;
using ExtraLife;
using ExtraLife.Entities;
using Xunit;

namespace ExtraLifeTests.IntegrationTests
{
    public class ExtraLifeApiClientTests
    {
        private ExtraLifeApiClient client;

        public ExtraLifeApiClientTests()
        {
            client = new ExtraLifeApiClient("https://try.donordrive.com/api");
        }

        [Fact]
        public async Task GetParticipantsAsync_WithValidParams_GetsParticipantList()
        {
            var response = await client.GetParticipantsAsync(1, 10);

            Assert.Equal(10, response.Count());
        }

        [Fact]
        public async Task GetParticipantsAsync_WithInvalidParams_ThrowsApiException()
        {
            await Assert.ThrowsAsync<ApiResponseException>(() => client.GetParticipantsAsync(-1, 10));
        }

        [Fact]
        public async Task GetParticipantAsync_WithValidId_ReturnsParticipantObject()
        {
            var response = await client.GetParticipantAsync(22130);

            Assert.Equal("Hugo Ibarra", response.DisplayName);
        }

        [Fact]
        public async Task GetParticipantAsync_WithInvalidId_ReturnsNull()
        {
            var response = await client.GetParticipantAsync(1);

            Assert.Null(response);
        }

        [Fact]
        public async Task GetParticipantDonationsAsync_WithValidParams_GetsDonationList()
        {
            var response = await client.GetParticipantDonationsAsync(22176, 1, 5);

            Assert.Equal(5, response.Count());
        }

        [Fact]
        public async Task GetParticipantDonationsAsync_WithInvalidId_ReturnsNull()
        {
            var response = await client.GetParticipantDonationsAsync(-1, 1, 5);

            Assert.Null(response);
        }

        [Fact]
        public async Task GetParticipantDonationsAsync_WithInvalidParams_ThrowsApiException()
        {
            await Assert.ThrowsAsync<ApiResponseException>(() => client.GetParticipantDonationsAsync(22176, -1, 5));
        }

        [Fact]
        public async Task GetParticipantDonorsAsync_WithValidParams_GetsDonorList()
        {
            var response = await client.GetParticipantDonorsAsync(22176, 1, 3);

            Assert.Equal(3, response.Count());
        }

        [Fact]
        public async Task GetParticipantDonorsAsync_WithInvalidId_ReturnsNull()
        {
            var response = await client.GetParticipantDonorsAsync(-1, 1, 3);

            Assert.Null(response);
        }

        [Fact]
        public async Task GetParticipantDonorsAsync_WithInvalidParams_ThrowsApiException()
        {
            await Assert.ThrowsAsync<ApiResponseException>(() => client.GetParticipantDonorsAsync(22176, -1, 3));
        }

        [Fact]
        public async Task GetParticipantActivitiesAsync_WithValidParams_GetsActivityList()
        {
            var response = await client.GetParticipantActivitiesAsync(22176);

            Assert.Equal(5, response.Count());
        }
    }
}