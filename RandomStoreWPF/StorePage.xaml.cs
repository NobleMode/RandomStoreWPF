using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            var games = GetGames(genre);
        }

        private List<String> GetRole()
        {
            using DBContext db = new DBContext();
            string userId = UserManager.CurrentUser?.UserId ?? "1";
            
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
                Margin = new Thickness(10, 0, 0, 0),
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
                    case "Publish":
                        
                        break;
                    case "Manage":
                        
                        break;
                    case "Dashboard":
                        
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
            List<Game> games = genre == "ALL" ? db.Games.ToList() : db.Games.Where(g => g.GameType != null && g.GameType.GameTypeName == genre).ToList();
            return games;
        }

        private void Profile_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO - Go to profile page
        }

        private void Logout_OnClick(object sender, RoutedEventArgs e)
        {
            UserManager.ClearCurrentUser();
            var loginPage = new MainWindow();
            loginPage.Show();
            Close();
        }
    }
}
