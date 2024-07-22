using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using RandomStoreWPF.Models;

namespace RandomStoreWPF;

public partial class CartPage : Window
{
    public CartPage()
    {
        InitializeComponent();
        GetGameInCart();
    }

    void GetGameInCart()
    {
        var user = UserManager.CurrentUser?.UserId;
        
        using DBContext db = new DBContext();
        var games = db.Carts
            .Include(c => c.Game)
            .ThenInclude(c => c.GameDeveloperNavigation)
            .Include(c => c.User)
            .Where(c => user != null && c.UserId == int.Parse(user))
            .ToList();

        CartDataGrid.ItemsSource = games;

        BtnCheckout.IsEnabled = (games.Count != 0);

        var total = games.Sum(c => c.Game.Price);
        Total.Text = total.ToString();
    }
    
    private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var button = sender as Button;
            var cartItem = button?.DataContext as Cart;

            using var db = new DBContext();
            // Log the generated SQL query
            var query = db.Carts
                .Where(c => cartItem != null && c.UserId == cartItem.UserId && c.CartId == cartItem.CartId)
                .ToString();

            var itemToRemove = db.Carts
                .FirstOrDefault(c => c.UserId == cartItem.UserId && c.CartId == cartItem.CartId);

            if (itemToRemove != null) db.Carts.Remove(itemToRemove);
            db.SaveChanges();

            GetGameInCart(); // Refresh the DataGrid
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    
    private void BtnCheckout_Click(object sender, RoutedEventArgs e)
    {
        var user = UserManager.CurrentUser?.UserId;
        using var db = new DBContext();
        var games = db.Carts
            .Include(c => c.Game)
            .ThenInclude(c => c.GameDeveloperNavigation)
            .Include(c => c.User)
            .Where(c => user != null && c.UserId == int.Parse(user))
            .ToList();

        foreach (var game in games)
        {
            db.Ownerships.Add(new Ownership
            {
                UserId = game.UserId,
                GameId = game.GameId,
                BuyDate = DateTime.Now
            });
            db.Carts.Remove(game);
        }
        db.SaveChanges();
        GetGameInCart();
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
        var storePage = new StorePage();
        storePage.Show();
        Close();
    }
}