using System;
using System.Collections.Generic;

namespace RandomStoreWPF.Models;

public partial class RoleTable
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string Perm { get; set; } = null!;

    public virtual ICollection<UserTable> UserTables { get; set; } = new List<UserTable>();
}
