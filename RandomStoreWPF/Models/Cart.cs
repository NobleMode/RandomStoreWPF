﻿using System;
using System.Collections.Generic;

namespace RandomStoreWPF.Models;

public partial class Cart
{
    public int UserId { get; set; }

    public int GameId { get; set; }

    public int CartId { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual UserTable User { get; set; } = null!;
}
