using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using RandomStoreWPF.Models;

namespace RandomStoreWPF
{
    /// <summary>
    /// Interaction logic for StorePage.xaml
    /// </summary>
    public partial class StorePage : Window
    {
        public StorePage()
        {
            InitializeComponent();
            GenerateGenreButtons();
            GenerateGameCards("ALL");
            GenerateAdditionalButton();
        }
        
        private void GenerateGenreButtons()
        {
            var genres = FetchGameGenresFromDatabase();
            
            if (genres == null)
            {
                MessageBox.Show("Failed to fetch genres from database");
                return;
            }
            
            genres.Add("ALL");
            
            foreach (var genre in genres)
            {
                var genreButton = new Button
                {
                    Content = genre,
                    Margin = new Thickness(0, 5, 0, 5),
                    Padding = new Thickness(10),
                    Name = "BtnGenre" + genre,
                };

                genreButton.Click += GenreButton_Click;
                GenreList.Children.Add(genreButton);
            }
        }

        private void GenreButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            
            var content = button?.Name.Substring("BtnGenre".Length);

            if (content != null) GenerateGameCards(content);
        }
        
        private void GenerateGameCards(string genre)
        {
            // Clear existing game cards
            GamesGrid.Children.Clear();
            GamesGrid.RowDefinitions.Clear();
            var games = GetGames(genre);
            int row = 0;

            for (int i = 0; i < games.Count; i += 2)
            {
                GamesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                
                var gameCardLeft = new GameCard { DataContext = games[i] };
                Grid.SetRow(gameCardLeft, row);
                Grid.SetColumn(gameCardLeft, 0);
                GamesGrid.Children.Add(gameCardLeft);

                if (i + 1 < games.Count)
                {
                    var gameCardRight = new GameCard { DataContext = games[i + 1] };
                    Grid.SetRow(gameCardRight, row);
                    Grid.SetColumn(gameCardRight, 1);
                    GamesGrid.Children.Add(gameCardRight);
                }

                row++;
            }
        }

        private List<String> GetRole()
        {
            using DBContext db = new DBContext();
            string userId = UserManager.CurrentUser?.UserId;
            
            int userRole = db.UserTables
                .FirstOrDefault(u => u.UserId == int.Parse(userId))?.RoleId ?? 1;
            
            return db.RoleTables
                .FirstOrDefault(
                    r => r.RoleId == userRole
                )?.Perm.Split(',').ToList() ?? new List<string>();
        }
        
        private void GenerateAdditionalButton()
        {
            List<String> role = GetRole();
            foreach (String r in role)
            {
                switch (r)
                {
                    case "publish":
                        GenerateButton("Publish");
                        break;
                    case "manage": 
                        GenerateButton("Manage");
                        break;
                    case "system": 
                        GenerateButton("Dashboard");
                        break;
                }
            }
        }
        
        private void GenerateButton(String content)
        {
            var barButton = new Button
            {
                Content = content,
                Padding = new Thickness(10, 5, 10, 5),
                Name = "BtnBar" + content,
                
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
            };

            barButton.Click += barButton_Click;
            BarButton.Children.Add(barButton);
        }
        
        private void barButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            
            var content = button?.Name.Substring("BtnBar".Length);

            if (content != null)
            {
                switch (content)
                {
                    case "Manage":
                        if (TitleProperty.ToString() != "GameDashboard")
                        {
                            var managePage = new GameDashboard();
                            managePage.Show();
                            Close();
                        }
                        
                        break;
                    case "Dashboard":
                        if (TitleProperty.ToString() != "Dashboard")
                        {
                            var dashboardPage = new Dashboard();
                            dashboardPage.Show();
                            Close();
                        }
                        
                        break;
                }
            }
        }

        private List<string?> FetchGameGenresFromDatabase()
        {
            using DBContext db = new DBContext();
            List<string?> genres = db.GameTypes.Select(gt => gt.GameTypeName).ToList();
            
            return genres;
        }
        
        private static List<Game> GetGames (string genre)
        {
            using DBContext db = new DBContext();
            List<Game> games = genre == "ALL" ? 
                db.Games.Include(g => g.GameType)
                    .Include(g => g.GameDeveloperNavigation)
                    .Where(g => g.GameStatus != false)
                    .ToList()
                : db.Games.Include(g => g.GameType)
                    .Include(g => g.GameDeveloperNavigation)
                    .Where(g => g.GameType != null && g.GameType.GameTypeName == genre)
                    .Where(g => g.GameStatus != false)
                    .ToList();
            return games;
        }

        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            UserManager.ClearCurrentUser();
            var loginPage = new MainWindow();
            loginPage.Show();
            Close();
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

        private void BtnLib_OnClick(object sender, RoutedEventArgs e)
        {
            if (TitleProperty.ToString() != "Library")
            {
                var libraryPage = new Library();
                libraryPage.Show();
                Close();
            }
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
}
