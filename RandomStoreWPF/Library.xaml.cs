using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using RandomStoreWPF.Models;

namespace RandomStoreWPF;

public partial class Library : Window
{
    public Library()
    {
        InitializeComponent();
        GetGameInLibrary();
    }
    
    private void BtnCart_OnClick(object sender, RoutedEventArgs e)
    {
        if (TitleProperty.ToString() != "Cart")
        {
            var cartPage = new CartPage();
            cartPage.Show();
            Close();
        }
    }
    
    private void BtnLib_OnClick(object sender, RoutedEventArgs e)
    {
        if (TitleProperty.ToString() != "Library")
        {
            var libraryPage = new Library();
            libraryPage.Show();
            Close();
        }
    }
    
    private void BtnStore_OnClick(object sender, RoutedEventArgs e)
    {
        if (TitleProperty.ToString() != "Store")
        {
            var storePage = new StorePage();
            storePage.Show();
            Close();
        }
    }

    private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
    {
        UserManager.ClearCurrentUser();
        var loginPage = new MainWindow();
        loginPage.Show();
        Close();
    }
    
    void GetGameInLibrary()
    {
        var user = UserManager.CurrentUser?.UserId;
        
        using DBContext db = new DBContext();
        var games = db.Ownerships
            .Include(o => o.Game)
            .ThenInclude(g => g.GameDeveloperNavigation)
            .Include(o => o.User)
            .Where(o => user != null && o.UserId == int.Parse(user))
            .ToList();
        
        GridList.ItemsSource = games;
    }

    private void GridList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var grid = GridList;
        var item = (grid.SelectedItem as Ownership)?.GameId;

        using DBContext db = new DBContext();
        var game = db.Games
            .Include(u => u.GameDeveloperNavigation).Include(game => game.GameType)
            .FirstOrDefault(g => g.GameId == item);

        if (game != null)
        {
            // Update the StackPanel elements with the selected game's details
            GameName.Text = game.GameName;
            GameGenre.Text = game.GameType?.GameTypeName;
            GameDev.Text = $"Made by {game.GameDeveloperNavigation?.FullName}";
            GameDesc.Text = game.GameDescription;
            GameSize.Text = $"{game.Size} MB";
            BtnDownload.Tag = game.Size; // Assuming you have a button for downloading
        }
    }

    private async void BtnDownload_OnClick(object sender, RoutedEventArgs e)
    {
        var gameSize = Convert.ToDouble(BtnDownload.Tag); // Assuming game size is stored in Tag
        var duration = gameSize * 10; // Adjust the multiplier as needed for simulation time
        var random = new Random();

        await Task.Run(() =>
        {
            double progress = 0;
            while (progress < 99)
            {
                progress += random.NextDouble() * 5; // Randomly increase progress by up to 5%
                progress = Math.Min(progress, 99); // Ensure progress does not exceed 99%
                Dispatcher.Invoke(() => PBDownload.Value = progress);
                Thread.Sleep(100); // Simulate time between updates
            }
        });

        PBDownload.Value = 99; // Ensure the progress bar shows 99%
        MessageBox.Show("Shit broke :)", "Download Almost Complete");
    }

    private void BtnProfile_Click(object sender, RoutedEventArgs e)
    {
        if (TitleProperty.ToString() != "Profile")
        {
            var profile = new Profile();
            profile.Show();
            Close();
        }
    }
}