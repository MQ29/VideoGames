using System;
using System.Collections.Generic;

namespace VideoGames.Core.Models;

public partial class GamePublisher
{
    public int Id { get; set; }

    public int? GameId { get; set; }

    public int? PublisherId { get; set; }

    public virtual Game? Game { get; set; }

    public virtual ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();

    public virtual Publisher? Publisher { get; set; }
}
