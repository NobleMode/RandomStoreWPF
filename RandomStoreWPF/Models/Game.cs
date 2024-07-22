using System;
using System.Collections.Generic;

namespace RandomStoreWPF.Models;

public partial class Game
{
    public int GameId { get; set; }

    public string? GameName { get; set; }

    public int? GameTypeId { get; set; }

    public string? GameDescription { get; set; }

    public int? GameDeveloper { get; set; }

    public bool? GameStatus { get; set; }

    public int? Price { get; set; }

    public double? Size { get; set; }

    public string? JsonDataLoc { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual UserTable? GameDeveloperNavigation { get; set; }

    public virtual GameType? GameType { get; set; }

    public virtual ICollection<Ownership> Ownerships { get; set; } = new List<Ownership>();

    public virtual ICollection<SubGame> SubGames { get; set; } = new List<SubGame>();
}
