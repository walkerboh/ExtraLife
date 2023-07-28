using AutoFixture.Xunit2;
using ExtraLife;
using ExtraLife.Entities;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ExtraLifeTests.UnitTests
{
    public class ExtraLifeApiClientTests
    {
        private readonly Uri baseUri = new("http://www.example.com");

        #region GetParticipantsAsync

        [Theory, AutoData]
        public async Task GetParticipantsAsync_WithValidParams_GetsParticipantList(IEnumerable<Participant> participants)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(participants))
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetParticipantsAsync(1, 10);

            Assert.Equal(participants.Count(), result.Count());
            mockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.EndsWith("/participants?offset=1&limit=10")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetParticipantsAsync_WithInvalidResponse_ThrowsApiException()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetParticipantsAsync(-1, 10));
        }

        #endregion GetParticipantsAsync

        #region GetAllParticipantsAsync

        [Theory, AutoData]
        public async Task GetAllParticipantsAsync_WithValidResponse_GetsParticipantList(IEnumerable<Participant> participants)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(participants))
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetAllParticipantsAsync();

            Assert.Equal(participants.Count(), result.Count());
            mockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.EndsWith("/participants")), ItExpr.IsAny<CancellationToken>());
        }

        [Theory, AutoData]
        public async Task GetAllParticipantsAsync_WithLinkHeader_CallsNextPages(IEnumerable<Participant> participants)
        {
            var content = new StringContent(JsonSerializer.Serialize(participants));

            var response = new HttpResponseMessage();
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Headers.Add("link", $"<{baseUri}/participants>;rel=\"next\"");
            response.Content = content;

            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = content });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetAllParticipantsAsync();

            mockMessageHandler.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.Contains("/participants")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetAllParticipantsAsync_WithInvalidResponse_ThrowsApiException()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetAllParticipantsAsync());
        }

        #endregion GetAllParticipantsAsync

        #region GetParticipantAsync

        [Theory, AutoData]
        public async Task GetParticipantAsync_WithValidParam_GetsParticipant(Participant participant)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(participant))
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetParticipantAsync(123);

            Assert.Equivalent(participant, result);
            mockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.EndsWith("/participants/123")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetParticipantAsync_WithNotFound_ReturnsNull()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetParticipantAsync(123);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetParticipantAsync_WithInvalidResponse_ThrowsApiException()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetParticipantAsync(123));
        }

        #endregion GetParticipantAsync

        #region GetParticipantDonationsAsync

        [Theory, AutoData]
        public async Task GetParticipantDonationsAsync_WithValidParams_GetsDonationList(IEnumerable<Donation> donations)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(donations))
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetParticipantDonationsAsync(123, 1, 10);

            Assert.Equal(donations.Count(), result.Count());
            mockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.EndsWith("/participants/123/donations?offset=1&limit=10")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetParticipantDonationsAsync_WithNotFound_ReturnsNull()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetParticipantDonationsAsync(123, 1, 10);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetParticipantDonationsAsync_WithInvalidResponse_ThrowsApiException()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetParticipantDonationsAsync(123, -1, 10));
        }

        #endregion GetParticipantDonationsAsync

        #region GetAllParticipantDonationsAsync

        [Theory, AutoData]
        public async Task GetAllParticipantDonationsAsync_WithValidResponse_GetsParticipantList(IEnumerable<Donation> donations)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(donations))
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetAllParticipantDonationsAsync(123);

            Assert.Equal(donations.Count(), result.Count());
            mockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.EndsWith("/participants/123/donations")), ItExpr.IsAny<CancellationToken>());
        }

        [Theory, AutoData]
        public async Task GetAllParticipantDonationsAsync_WithLinkHeader_CallsNextPages(IEnumerable<Donation> donations)
        {
            var content = new StringContent(JsonSerializer.Serialize(donations));

            var response = new HttpResponseMessage();
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Headers.Add("link", $"<{baseUri}/participants/123/donations>;rel=\"next\"");
            response.Content = content;

            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = content });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetAllParticipantDonationsAsync(123);

            mockMessageHandler.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.Contains("/participants/123/donations")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetAllParticipantDonationsAsync_WithNotFound_ReturnsNull()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetAllParticipantDonationsAsync(123);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllParticipantDonationsAsync_WithInvalidResponse_ThrowsApiException()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetAllParticipantDonationsAsync(123));
        }

        #endregion GetAllParticipantDonationsAsync

        #region GetParticipantDonorsAsync

        [Theory, AutoData]
        public async Task GetParticipantDonorsAsync_WithValidParams_GetsDonationList(IEnumerable<Donor> donors)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(donors))
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetParticipantDonorsAsync(123, 1, 10);

            Assert.Equal(donors.Count(), result.Count());
            mockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.EndsWith("/participants/123/donors?offset=1&limit=10")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetParticipantDonorsAsync_WithNotFound_ReturnsNull()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetParticipantDonorsAsync(123, 1, 10);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetParticipantDonorsAsync_WithInvalidResponse_ThrowsApiException()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetParticipantDonorsAsync(123, -1, 10));
        }

        #endregion GetParticipantDonorsAsync

        #region GetAllParticipantDonorsAsync

        [Theory, AutoData]
        public async Task GetAllParticipantDonorsAsync_WithValidResponse_GetsParticipantList(IEnumerable<Donor> donors)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(donors))
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetAllParticipantDonorsAsync(123);

            Assert.Equal(donors.Count(), result.Count());
            mockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.EndsWith("/participants/123/donors")), ItExpr.IsAny<CancellationToken>());
        }

        [Theory, AutoData]
        public async Task GetAllParticipantDonorsAsync_WithLinkHeader_CallsNextPages(IEnumerable<Donor> donors)
        {
            var content = new StringContent(JsonSerializer.Serialize(donors));

            var response = new HttpResponseMessage();
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Headers.Add("link", $"<{baseUri}/participants/123/donors>;rel=\"next\"");
            response.Content = content;

            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = content });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetAllParticipantDonorsAsync(123);

            mockMessageHandler.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.Contains("/participants/123/donors")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetAllParticipantDonorsAsync_WithNotFound_ReturnsNull()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetAllParticipantDonorsAsync(123);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllParticipantDonorsAsync_WithInvalidResponse_ThrowsApiException()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetAllParticipantDonorsAsync(123));
        }

        #endregion GetAllParticipantDonorsAsync

        #region GetAllParticipantActivitiesAsync

        [Theory, AutoData]
        public async Task GetAllParticipantActivitiesAsync_WithValidResponse_GetsParticipantList(IEnumerable<Activity> activities)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(activities))
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetAllParticipantActivitiesAsync(123);

            Assert.Equal(activities.Count(), result.Count());
            mockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.EndsWith("/participants/123/activity")), ItExpr.IsAny<CancellationToken>());
        }

        [Theory, AutoData]
        public async Task GetAllParticipantActivitiesAsync_WithLinkHeader_CallsNextPages(IEnumerable<Activity> activities)
        {
            var content = new StringContent(JsonSerializer.Serialize(activities));

            var response = new HttpResponseMessage();
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Headers.Add("link", $"<{baseUri}/participants/123/activity>;rel=\"next\"");
            response.Content = content;

            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = content });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetAllParticipantActivitiesAsync(123);

            mockMessageHandler.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsoluteUri.Contains("/participants/123/activity")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetAllParticipantActivitiesAsync_WithNotFound_ReturnsNull()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            var result = await sut.GetAllParticipantActivitiesAsync(123);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllParticipantActivitiesAsync_WithInvalidResponse_ThrowsApiException()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            var client = new HttpClient(mockMessageHandler.Object);
            client.BaseAddress = baseUri;

            var sut = new ExtraLifeApiClient(client);

            await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetAllParticipantActivitiesAsync(123));
        }

        #endregion GetAllParticipantActivitiesAsync
    }
}
