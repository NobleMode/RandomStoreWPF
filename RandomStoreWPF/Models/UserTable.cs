using System;
using System.Collections.Generic;

namespace RandomStoreWPF.Models;

public partial class UserTable
{
    public int UserId { get; set; }

    public string? FullName { get; set; }

    public int? RoleId { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual ICollection<Ownership> Ownerships { get; set; } = new List<Ownership>();

    public virtual RoleTable? Role { get; set; }

    public virtual UserCode? UserCode { get; set; }

    public virtual ICollection<UserSecurity> UserSecurities { get; set; } = new List<UserSecurity>();
}
