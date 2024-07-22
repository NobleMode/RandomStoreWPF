using System;
using System.Collections.Generic;

namespace RandomStoreWPF.Models;

public partial class UserCode
{
    public int UserId { get; set; }

    public string Code { get; set; } = null!;

    public bool? Downloaded { get; set; }

    public virtual UserTable User { get; set; } = null!;
}
