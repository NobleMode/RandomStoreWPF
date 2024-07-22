using Microsoft.EntityFrameworkCore;
using RandomStoreWPF.Models;
using System.Windows;
using System.Windows.Controls;

namespace RandomStoreWPF;

public partial class GameDashboard : Window
{
    public GameDashboard()
    {
        InitializeComponent();
        GetAllGame();
        GetSidePanel();
        ComboGenre();
    }

    private void ComboGenre()
    {
        using DBContext db = new DBContext();
        var genres = db.GameTypes.ToList();

        foreach (var genre in genres)
        {
            cmbGenre.Items.Add(genre.GameTypeName);
        }
    }

    private void GetAllGame()
    {
        var roles = GetRole();
        
        using DBContext db = new DBContext();
        
        if (roles == "admin")
        {
            var games = db.Games.Include(g => g.GameDeveloperNavigation).ToList();
            DataGridGames.ItemsSource = games;
        }
        
        if (roles == "dev")
        {
            var userId = UserManager.CurrentUser?.UserId;
            var games = db.Games
                .Include(g => g.GameDeveloperNavigation)
                .Where(g => g.GameDeveloperNavigation.UserId == int.Parse(userId))
                .ToList();
            DataGridGames.ItemsSource = games;
        }
    }

    private void GetSidePanel()
    {
        var roles = GetRole();

        if (roles == "admin")
        {
            SPGameDev.Visibility = Visibility.Collapsed;
            SPAdmin.Visibility = Visibility.Visible;
        }

        if (roles == "dev")
        {
            SPGameDev.Visibility = Visibility.Visible;
            SPAdmin.Visibility = Visibility.Collapsed;
        }
    }

    private String GetRole()
    {
        using DBContext db = new DBContext();
        string userId = UserManager.CurrentUser?.UserId ?? "1";

        int userRole = db.UserTables
            .FirstOrDefault(u => u.UserId == int.Parse(userId))?.RoleId ?? 1;

        return db.RoleTables.FirstOrDefault(r => r.RoleId == userRole)?.RoleName ?? "";

    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        // Open the Store Page window
        new StorePage().Show();
        // Close the current Game Dashboard window
        this.Close();
    }

    private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
    {
        if (BtnEdit.Tag == null)
        {
            MessageBox.Show("Please select a game to edit.");
            return;
        }

        int gameId = (int)BtnEdit.Tag;
        using (DBContext db = new DBContext())
        {
            var game = db.Games.FirstOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                MessageBox.Show("Game not found.");
                return;
            }

            game.GameName = txtGameName.Text;
            game.GameType = db.GameTypes.FirstOrDefault(gt => gt.GameTypeName == cmbGenre.SelectedItem.ToString());
            game.Price = int.Parse(txtPrice.Text);
            game.Size = int.Parse(txtSize.Text);
            game.GameDescription = txtDescription.Text;
            game.GameStatus = cmbStatus.SelectedItem.ToString() == "Active" ? true : false;

            db.SaveChanges();
            GetAllGame();
        }

        MessageBox.Show("Game updated successfully.");
    }

    private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
    {
        using DBContext db = new DBContext();
        var game = new Game
        {
            GameName = txtGameName.Text,
            GameType = db.GameTypes.FirstOrDefault(gt => gt.GameTypeName == cmbGenre.SelectedItem.ToString()),
            Price = int.Parse(txtPrice.Text),
            Size = double.Parse(txtSize.Text),
            GameDescription = txtDescription.Text,
            GameDeveloper = db.UserTables.FirstOrDefault(u => u.UserName == txtDeveloper.Text)?.UserId,
            GameStatus = true
        };

        db.Games.Add(game);
        db.SaveChanges();
        GetAllGame();
    }

    private void BtnDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var game = SPAdmin.Visibility == Visibility.Visible ? txtAdminGameName.Text : txtGameName.Text;
        
        using DBContext db = new DBContext();
        
        var gameToDelete = db.Games.FirstOrDefault(g => g.GameName == game);
        if (gameToDelete != null)
        {
            db.Games.Remove(gameToDelete);
            db.SaveChanges();
            GetAllGame();
        }
    }

    private void BtnDelist_OnClick(object sender, RoutedEventArgs e)
    {
        var gameId = (int)((Button)sender).Tag;
        
        using (DBContext db = new DBContext())
        {
            var game = db.Games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                game.GameStatus = game.GameStatus == true ? false : true;
                db.SaveChanges();
            }
        }
    }

    private void DataGridGames_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var dataGrid = DataGridGames;
        var id = (dataGrid.SelectedItem as Game)?.GameId;
        
        using DBContext db = new DBContext();
        var game = db.Games
            .Include(g => g.GameDeveloperNavigation)
            .Include(g => g.GameType)
            .FirstOrDefault(g => g.GameId == id);
        
        var role = GetRole();
        if (role == "admin") {AdminList(game);}
        if (role == "dev") {DevList(game);}
    }

    private void DevList(Game game)
    {
        txtGameName.Text = game.GameName;    
        cmbGenre.SelectedItem = game.GameType?.GameTypeName;
        txtPrice.Text = game.Price.ToString();
        txtSize.Text = game.Size.ToString();
        txtDescription.Text = game.GameDescription;
        txtDeveloper.Text = game.GameDeveloperNavigation?.UserName;
        cmbStatus.SelectedItem = game.GameStatus == true ? "Active" : "Delisted";
        BtnEdit.Tag = game.GameId;
    }
    
    private void AdminList(Game game)
    {
        txtAdminGameName.Text = game.GameName;
        txtAdminGenre.Text = game.GameType?.GameTypeName;
        txtAdminDeveloper.Text = game.GameDeveloperNavigation?.UserName;
        txtAdminPrice.Text = game.Price.ToString();
        txtAdminSize.Text = game.Size.ToString();
        txtAdminDescription.Text = game.GameDescription;
        txtAdminStatus.Text = game.GameStatus == true ? "Active" : "Delisted";
        BtnDelist.Tag = game.GameId;
    }
}