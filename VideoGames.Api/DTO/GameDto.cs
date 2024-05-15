namespace VideoGames.Api.DTO
{
    public class GameDto
    {
        public int Id { get; set; }
        public string GameName { get; set; }
        public string PublisherName { get; set; }
        public string GamePlatform { get; set; }
        public int? ReleaseYear { get; set; }
    }
}
