using System;
using System.Collections.Generic;

namespace RandomStoreWPF.Models;

public partial class SecurityQuestion
{
    public int QuestionId { get; set; }

    public string? QuestionText { get; set; }

    public int? Severity { get; set; }

    public virtual ICollection<UserSecurity> UserSecurities { get; set; } = new List<UserSecurity>();
}
