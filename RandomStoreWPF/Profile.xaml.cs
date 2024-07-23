using RandomStoreWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
using Path = System.IO.Path;

namespace RandomStoreWPF
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        public Profile()
        {
            InitializeComponent();
        }

        private void BtnPersonal_Click(object sender, RoutedEventArgs e)
        {
            var userId = UserManager.CurrentUser?.UserId;

            using var db = new DBContext();
            var user = db.UserTables.FirstOrDefault(u => u.UserId == int.Parse(userId));
            var userCode = db.UserCodes.FirstOrDefault(u => u.UserId == int.Parse(userId));
            OutputUserInfoToFile(user.UserName, userCode.Code);
        }

        public void OutputUserInfoToFile(string userName, string userCode)
        {
            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"{userName}_PersonalCode.txt";
            
            string filePath = Path.Combine(downloadsPath, fileName);
            
            string content = $"Personal Code for User: {userName}\n" +
                             $"Code is: {userCode}\n" +
                             $"Generated at Date: {DateTime.Now}";
            
            File.WriteAllText(filePath, content);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            // Open the Store Page window
            new StorePage().Show();
            // Close the current Game Dashboard window
            this.Close();
        }
    }
}
