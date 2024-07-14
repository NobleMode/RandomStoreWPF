using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RandomStoreWPF.Models;

namespace RandomStoreWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Login_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        MainTabControl.SelectedItem = RegisterTab;
        e.Handled = true;
    }

    private void Register_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        MainTabControl.SelectedItem = LoginTab;
        e.Handled = true;
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        var username = TbxUserLogin.Text;
        var password = TbxPassLogin.Password;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            TbkLoginError.Text = "Please fill in all fields";
            return;
        }

        var user = SqlQuery(username, password);
        if (user == null)
        {
            TbkLoginError.Text = "Password Invalid / User don't exist";
            return;
        }

        TbkLoginError.Text = null;
        UserManager.SetCurrentUser(user.UserId.ToString(), user.UserName, user.Email);
    
        // Move to main store page
        MoveToMain();
    }
    
    private void BtnRegister_OnClick(object sender, RoutedEventArgs e)
    {
        var username = TbxUserRegister.Text;
        var fullname = TbxNameRegister.Text;
        var email = TbxEmailRegister.Text;
        var password = TbxPassRegister.Text;
        
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fullname) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            TbkRegisterError.Text = "Please fill in all fields";
            return;
        }

        var user = SqlQuery(username, password);
        if (user != null)
        {
            TbkRegisterError.Text = "User already exist. Please Login instead!";
            return;
        }
        
        var newUser = new UserTable
        {
            UserName = username,
            FullName = fullname,
            Email = email,
            Password = password,
            RoleId = 1
        };
        InsertSql(newUser);
        SqlQuery(newUser.UserName, newUser.Password);
        UserManager.SetCurrentUser(newUser.UserId.ToString(), newUser.UserName, newUser.Email);
        //Move to Main Page
        MoveToMain();
    }
    
    private UserTable? SqlQuery(string username, string password)
    {
        using var db = new DBContext();
        var user = db.UserTables.FirstOrDefault(u => u.UserName == username);
    
        if (user == null || password != user.Password)
        {
            return null;
        }
    
        return user;
    }

    
    private void InsertSql(UserTable newUser)
    {
        using var db = new DBContext();
        db.UserTables.Add(newUser); 
        db.SaveChanges();
    }

    private void BtnGuest_OnClick(object sender, RoutedEventArgs e)
    {
        // Move to main store page
        MoveToMain();
    }

    private void MoveToMain()
    {
        var storePage = new StorePage();
        storePage.Show();
        this.Close();
    }
}