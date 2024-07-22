using System;
using System.Collections.Generic;

namespace RandomStoreWPF.Models;

public partial class Ownership
{
    public int UserId { get; set; }

    public int GameId { get; set; }

    public int OwnershipId { get; set; }

    public DateTime? BuyDate { get; set; }

    public string? PersonalCode { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual UserTable User { get; set; } = null!;
}
