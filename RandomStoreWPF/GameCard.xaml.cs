using System.Windows;
using System.Windows.Controls;
using RandomStoreWPF.Models;

namespace RandomStoreWPF;

public partial class GameCard : UserControl
{
    public GameCard()
    {
        InitializeComponent();
    }

    private void BtnCartAdd_OnClick(object sender, RoutedEventArgs e)
    {
        var user = UserManager.CurrentUser?.UserId;
        var gameId = (int)BtnCartAdd.Tag;
        
        using DBContext db = new DBContext();
        if (IsGameInCart(gameId))
        {
            MessageBox.Show("Game is already in cart");
            return;
        }

        if (user != null)
        {
            db.Carts.Add(new Cart
            {
                UserId = int.Parse(user),
                GameId = gameId
            });
            db.SaveChanges();
        }
    }
    
    private bool IsGameInCart(int gameId)
    {
        using DBContext db = new DBContext();
        var user = UserManager.CurrentUser?.UserId;
        return db.Carts.Any(c => user != null && c.UserId == int.Parse(user) && c.GameId == gameId);
    }
}