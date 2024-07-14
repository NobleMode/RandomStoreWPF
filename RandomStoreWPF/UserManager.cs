namespace RandomStoreWPF;

public class CurrentUser
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
}

public static class UserManager
{
    private static CurrentUser? _cu;

    public static CurrentUser? CurrentUser
    {
        get => _cu;
        set => _cu = value;
    }
    
    public static void SetCurrentUser(string? userId, string? userName, string? email)
    {
        _cu = new CurrentUser
        {
            UserId = userId,
            UserName = userName,
            Email = email
        };
    }
    
    public static void ClearCurrentUser()
    {
        SetCurrentUser(null, null, null);
    }
}