using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace MinimalApiTests
{
    public class ApiTests : IAsyncLifetime
    {
        private HttpClient? _httpClient;
        private HttpResponseMessage? _cachedResponse;
        private const string BaseUrl = "https://jsonplaceholder.typicode.com/posts";

        private readonly Post _expectedPost = new Post
        {
            UserId = 8,
            Id = 73,
            Title = "consequuntur deleniti eos quia temporibus ab aliquid at",
            Body = "voluptatem cumque tenetur consequatur expedita ipsum nemo quia explicabo\naut eum minima consequatur\ntempore cumque quae est et\net in consequuntur voluptatem voluptates aut"
        };

        public async Task InitializeAsync()
        {
            _httpClient = new HttpClient();
            _cachedResponse = await _httpClient.GetAsync(BaseUrl);
        }

        [Fact]
        public void API_Response_StatusCode()
        {
            Assert.NotNull(_cachedResponse);
            Assert.Equal(HttpStatusCode.OK, _cachedResponse?.StatusCode);
        }

        [Fact]
        public async Task Get_Posts_ShouldContainExpectedPost()
        {
            Assert.NotNull(_cachedResponse);
            var responseContent = await _cachedResponse.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));

            using (JsonDocument doc = JsonDocument.Parse(responseContent))
            {
                Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);

                var posts = doc.RootElement.EnumerateArray()
                    .Select(p => new Post
                    {
                        UserId = p.GetProperty("userId").GetInt32(),
                        Id = p.GetProperty("id").GetInt32(),
                        Title = p.GetProperty("title").GetString() ?? string.Empty,
                        Body = p.GetProperty("body").GetString() ?? string.Empty
                    })
                    .ToList();

                var foundPost = posts.SingleOrDefault(p => p.Id == _expectedPost.Id);

                Assert.NotNull(foundPost);
                Assert.Equal(_expectedPost.UserId, foundPost.UserId);
                Assert.Equal(_expectedPost.Title, foundPost.Title);
                Assert.Equal(_expectedPost.Body, foundPost.Body);
            }
        }

        public async Task DisposeAsync()
        {
            _httpClient?.Dispose();
            await Task.CompletedTask;
        }

        // Define the Post class matching the JSON structure
        private class Post
        {
            public int UserId { get; set; }
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
        }
    }
}
