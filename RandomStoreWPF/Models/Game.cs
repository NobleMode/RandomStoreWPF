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

    public int? GameStatus { get; set; }

    public int? Price { get; set; }

    public virtual UserTable? GameDeveloperNavigation { get; set; }

    public virtual GameType? GameType { get; set; }

    public virtual ICollection<UserTable> Users { get; set; } = new List<UserTable>();
}
