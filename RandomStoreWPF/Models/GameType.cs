using System;
using System.Collections.Generic;

namespace RandomStoreWPF.Models;

public partial class GameType
{
    public int GameTypeId { get; set; }

    public string? GameTypeName { get; set; }

    public string? GameTypeDescription { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual ICollection<SubGame> SubGames { get; set; } = new List<SubGame>();
}
