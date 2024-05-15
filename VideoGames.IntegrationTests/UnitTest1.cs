using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using VideoGames.Api.DTO;
using VideoGames.Core.Models;
using VideoGames.Infrasctructure;
using VideoGames.IntegrationTests;

public class VideoGamesControllerTests : IClassFixture<VideoGamesTestFactory<Program>>
{
    private readonly VideoGamesTestFactory<Program> _factory;
    private readonly HttpClient _http;
    private readonly VideoGamesContext _context;

    public VideoGamesControllerTests(VideoGamesTestFactory<Program> factory)
    {
        _factory = factory;
        _http = factory.CreateClient();
        using (var scope = _factory.Services.CreateScope())
        {
            _context = scope.ServiceProvider.GetService<VideoGamesContext>();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var genre = new Genre { Id = 1, GenreName = "Action" };
            var publisher = new Publisher { Id = 1, PublisherName = "Test Publisher" };
            var platform = new Platform { Id = 1, PlatformName = "PC" };

            var game = new Game
            {
                Id = 1,
                GameName = "Test Game",
                Genre = genre,
                GamePublishers = new List<GamePublisher>
                {
                    new GamePublisher
                    {
                        Publisher = publisher,
                        GamePlatforms = new List<GamePlatform>
                        {
                            new GamePlatform
                            {
                                Platform = platform,
                                ReleaseYear = 2020
                            }
                        }
                    }
                }
            };

            _context.Games.Add(game);
            _context.Genres.Add(genre);
            _context.Publishers.Add(publisher);
            _context.Platforms.Add(platform);
            _context.SaveChanges();
        }
    }

    [Fact]
    public async Task GetGamesByGenre_ReturnsGames_WhenGenreExists()
    {
        // Act
        var response = await _http.GetAsync("/api/games?genre=Action");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var games = JsonSerializer.Deserialize<List<GameDto>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.Single(games);
        Assert.Equal("Test Game", games.First().GameName);
    }

    [Fact]
    public async Task GetGamesByGenre_ReturnsNotFound_WhenGenreDoesNotExist()
    {
        // Act
        var response = await _http.GetAsync("/api/games?genre=UnknownGenre");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
