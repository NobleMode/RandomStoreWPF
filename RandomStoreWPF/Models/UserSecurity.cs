using System;
using System.Collections.Generic;

namespace RandomStoreWPF.Models;

public partial class UserSecurity
{
    public int UserId { get; set; }

    public int QuestionId { get; set; }

    public string? Answer { get; set; }

    public virtual SecurityQuestion Question { get; set; } = null!;

    public virtual UserTable User { get; set; } = null!;
}
