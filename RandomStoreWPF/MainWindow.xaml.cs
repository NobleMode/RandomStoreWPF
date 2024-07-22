using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Navigation;
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

        if (user.Status == false)
        {
            TbkLoginError.Text = "User is banned";
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
        var confirmPassword = TbxSamePassRegister.Text;

        if (string.IsNullOrWhiteSpace(username) 
            || string.IsNullOrWhiteSpace(fullname) 
            || string.IsNullOrWhiteSpace(email) 
            || string.IsNullOrWhiteSpace(password) 
            || string.IsNullOrWhiteSpace(confirmPassword))
        {
            TbkRegisterError.Text = "Please fill in all fields";
            return;
        }

        if (!CheckEmail(email)) return;
        if (!CheckPassword(password, confirmPassword)) return;


        if (username.Length < 4 || username.Length > 16)
        {
            TbkLoginError.Text = "Username must be between 4 and 16 characters";
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
            RoleId = 1,
            Status = true,
        };


        SecurityQuestionsModal securityQuestionsModal = new SecurityQuestionsModal();
        var result = securityQuestionsModal.ShowDialog();
        if (result == true)
        {
            InsertSql(newUser);
            InsertUserCode(newUser, GenerateRandomString(30));
            var list = securityQuestionsModal.UserSecurityQuestions;
            foreach (var a in list)
            {
                InsertSecurityQuestions(newUser.UserId, a.q, a.a);
            }
            UserManager.SetCurrentUser(newUser.UserId.ToString(), newUser.UserName, newUser.Email);
            //Move to Main Page
            MoveToMain();
        }
    }

    private bool CheckEmail(string email)
    {
        if (!email.Contains("@"))
        {
            TbkRegisterError.Text = "Invalid Email";
            return false;
        }

        return true;
    }

    private bool CheckPassword(string password, string confirmPass)
    {
        if (password.Length < 8 || password.Length > 32)
        {
            TbkRegisterError.Text = "Password must be between 8 and 32 characters";
            return false;
        }

        if (!Regex.IsMatch(password, @"\d"))
        {
            TbkRegisterError.Text = "Password must contain at least 1 digit";
            return false;
        }

        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            TbkRegisterError.Text = "Password must contain at least 1 lowercase letter";
            return false;
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            TbkRegisterError.Text = "Password must contain at least 1 uppercase letter";
            return false;
        }

        if (!Regex.IsMatch(password, @"[!@#$%^&*]"))
        {
            TbkRegisterError.Text = "Password must contain at least 1 special character (!@#$%^&*)";
            return false;
        }

        if (password != confirmPass)
        {
            TbkRegisterError.Text = "Password and Confirm Password must be the same";
            return false;
        }

        return true;
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
    
    private void InsertSecurityQuestions(int userId, SecurityQuestion question, string answer)
    {
        using var db = new DBContext();
        var securityQuestion = new UserSecurity
        {
            UserId = userId,
            QuestionId = question.QuestionId,
            Answer = answer
        };
        db.UserSecurities.Add(securityQuestion);
        db.SaveChanges();
    }
    
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var randomString = new char[length];

        for (int i = 0; i < length; i++)
        {
            randomString[i] = chars[random.Next(chars.Length)];
        }

        return new String(randomString);
    }

    public void InsertUserCode(UserTable user, String code)
    {
        using DBContext db = new DBContext();
        var userCode = new UserCode
        {
            UserId = user.UserId,
            Code = code,
            Downloaded = false
        };
        db.UserCodes.Add(userCode);
        db.SaveChanges();
    }

    private void MoveToMain()
    {
        var storePage = new StorePage();
        storePage.Show();
        this.Close();
    }

    private void BtnForget_OnClick(object sender, RoutedEventArgs e)
    {
        using var db = new DBContext();
        var email = TbxEmailForget.Text;
        var code = TbxCodeForget.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
        {
            TbkForgetError.Text = "Please fill in all fields";
            return;
        }

        var user = db.UserTables.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            TbkForgetError.Text = "Email not found";
            return;
        }

        var userCode = db.UserCodes.FirstOrDefault(u => u.UserId == user.UserId && u.Code == code);
        if (userCode == null)
        {
            TbkForgetError.Text = "Code not found";
            return;
        }
        if (userCode.Downloaded == true)
        {
            TbkForgetError.Text = "Code already used";
            return;
        }

        TbkForgetError.Text = null;
        VerifiedSP.Visibility = Visibility.Visible;
        TbxEmailForget.IsEnabled = false;
        TbxCodeForget.IsEnabled = false;
    }

    private void BtnChangePass_Click(object sender, RoutedEventArgs e)
    {
        var newPass = TbxNewPassForget.Text;
        var confirmPass = TbxSameNewPassForget.Text;

        if (string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirmPass))
        {
            TbkForgetSuccess.Text = "Please fill in all fields";
            return;
        }

        if (!CheckPassword(newPass, confirmPass)) return;

        using var db = new DBContext();
        var email = TbxEmailForget.Text;
        
        var user = db.UserTables.FirstOrDefault(u => u.Email == email);
        user.Password = newPass;

        var userCode = db.UserCodes.FirstOrDefault(u => u.UserId == user.UserId);
        userCode.Code = GenerateRandomString(30);
        userCode.Downloaded = false;

        db.SaveChanges();

        MessageBox.Show("Password Changed Successfully. \n New Code has been generated. \n Please redownload them from Profile Page");

        MoveToMain();
    }
}