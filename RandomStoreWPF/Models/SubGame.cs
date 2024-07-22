using System;
using System.Collections.Generic;

namespace RandomStoreWPF.Models;

public partial class SubGame
{
    public int GameId { get; set; }

    public int SubId { get; set; }

    public int? GameTypeId { get; set; }

    public int? Price { get; set; }

    public bool? Status { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual GameType? GameType { get; set; }
}
