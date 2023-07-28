using ExtraLife.Utilities;
using Xunit;

namespace ExtraLifeTests.UnitTests.Utilities
{
    public class LinkHeaderTests
    {
        private const string url = "https://www.example.com";

        [Fact]
        public void ParseLinkHeader_WithNullParam_ShouldReturnNull()
        {
            var linkHeader = LinkHeader.ParseLinkHeader(null);
            Assert.Null(linkHeader);
        }

        [Fact]
        public void ParseLinkHeader_WithEmptyStringParam_ShouldReturnNull()
        {
            var linkHeader = LinkHeader.ParseLinkHeader(string.Empty);
            Assert.Null(linkHeader);
        }

        [Fact]
        public void ParseLinkHeader_WithValidParams_ShouldCorrectlyParseFirstLink()
        {
            var linkHeader = LinkHeader.ParseLinkHeader($"<{url}>;rel=\"first\"");
            Assert.Equal(url, linkHeader.FirstLink);
            Assert.Null(linkHeader.NextLink);
            Assert.Null(linkHeader.PrevLink);
            Assert.Null(linkHeader.LastLink);
        }

        [Fact]
        public void ParseLinkHeader_WithValidParams_ShouldCorrectlyParseNextLink()
        {
            var linkHeader = LinkHeader.ParseLinkHeader($"<{url}>;rel=\"next\"");
            Assert.Equal(url, linkHeader.NextLink);
            Assert.Null(linkHeader.FirstLink);
            Assert.Null(linkHeader.PrevLink);
            Assert.Null(linkHeader.LastLink);
        }

        [Fact]
        public void ParseLinkHeader_WithValidParams_ShouldCorrectlyParsePrevLink()
        {
            var linkHeader = LinkHeader.ParseLinkHeader($"<{url}>;rel=\"prev\"");
            Assert.Equal(url, linkHeader.PrevLink);
            Assert.Null(linkHeader.FirstLink);
            Assert.Null(linkHeader.NextLink);
            Assert.Null(linkHeader.LastLink);
        }

        [Fact]
        public void ParseLinkHeader_WithValidParams_ShouldCorrectlyParseLastLink()
        {
            var linkHeader = LinkHeader.ParseLinkHeader($"<{url}>;rel=\"last\"");
            Assert.Equal(url, linkHeader.LastLink);
            Assert.Null(linkHeader.FirstLink);
            Assert.Null(linkHeader.NextLink);
            Assert.Null(linkHeader.PrevLink);
        }

        [Fact]
        public void ParseLinkHeader_WithValidParams_ShouldCorrectlyParseMultipleLinks()
        {
            var linkHeader = LinkHeader.ParseLinkHeader($"<{url}>;rel=\"next\",<{url}>;rel=\"last\"");
            Assert.Equal(url, linkHeader.NextLink);
            Assert.Equal(url, linkHeader.LastLink);
            Assert.Null(linkHeader.FirstLink);
            Assert.Null(linkHeader.PrevLink);
        }

        [Fact]
        public void ParseLinkHeader_WithValidParams_IgnoresInvalidLinks()
        {
            var linkHeader = LinkHeader.ParseLinkHeader($"<{url}>;rel=\"random\"");
            Assert.Null(linkHeader.FirstLink);
            Assert.Null(linkHeader.NextLink);
            Assert.Null(linkHeader.PrevLink);
            Assert.Null(linkHeader.LastLink);
        }
    }
}
