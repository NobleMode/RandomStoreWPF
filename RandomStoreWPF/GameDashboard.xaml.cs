using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using RandomStoreWPF.Models;
using System.IO;
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

        if (game.GameType?.GameTypeName == null)
        {
            cmbGenre.SelectedIndex = 0;
        }
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

        if (game.GameType?.GameTypeName == null)
        {
            txtAdminGameName.Text = "";
        }
    }

    private string BrowseImageFile()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
        openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }

        return null;
    }

    private string browseZipFile()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Zip files (*.zip)|*.zip|All files (*.*)|*.*";
        openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }

        return null;
    }

    private void CopyImageToNewLocation(string sourceFilePath, string destinationDirectory)
    {
        try
        {
            // Ensure the destination directory exists
            Directory.CreateDirectory(destinationDirectory);

            // Get the file name to use in the destination directory
            string fileName = Path.GetFileName(sourceFilePath);
            string destinationFilePath = Path.Combine(destinationDirectory, fileName);

            // Copy the file
            File.Copy(sourceFilePath, destinationFilePath, true); // true allows overwriting

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error copying image: {ex.Message}");
        }
    }

    private void BtnUploadAndCopyImage_Click(object sender, RoutedEventArgs e)
    {
        string sourceImagePath = BrowseImageFile();
        if (!string.IsNullOrEmpty(sourceImagePath))
        {
            // Specify the directory where you want to copy the image
            string destinationDirectory = @"C:\path\to\your\destination\directory";
            CopyImageToNewLocation(sourceImagePath, destinationDirectory);
        }
    }

    private void BtnUploadAndCopyZip_Click(object sender, RoutedEventArgs e)
    {
        string sourceZipPath = browseZipFile();
        if (!string.IsNullOrEmpty(sourceZipPath))
        {
            // Specify the directory where you want to copy the zip file
            string destinationDirectory = @"C:\path\to\your\destination\directory";
            CopyImageToNewLocation(sourceZipPath, destinationDirectory);
        }
    }

}