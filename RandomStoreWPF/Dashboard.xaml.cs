using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using RandomStoreWPF.Models;

namespace RandomStoreWPF;

public partial class Dashboard : Window
{
    public Dashboard()
    {
        InitializeComponent();
        GetAllUser();
        GetAllRoleComboBox();
    }

    private void GetAllRoleComboBox()
    {
        using DBContext db = new DBContext();
        var roles = db.RoleTables.ToList();

        foreach (var r in roles)
        {
            cmbRole.Items.Add(r.RoleName);
        }
    }

    private void GetAllUser()
    {
        using DBContext db = new DBContext();
        var users = db.UserTables.Include(u => u.Role).ToList();
        DataGridUser.ItemsSource = users;
    }

    private void DataGridUser_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var user = (DataGridUser.SelectedItem as UserTable)?.UserId;
        
        if (user == null) return;
        
        using DBContext db = new DBContext();
        var selectedUser = db.UserTables
            .Include(u => u.Role)
            .FirstOrDefault(u => u.UserId == user);

        setTableSide(selectedUser);
    }
    
    private void setTableSide(UserTable user)
    {
        txtUserID.Text = user.UserId.ToString();
        txtFullName.Text = user.FullName;
        txtUsername.Text = user.UserName;
        cmbRole.SelectedItem = user.Role.RoleName;
        txtEmail.Text = user.Email;
        if (user.Status == true)
        {
            rbActive.IsChecked = true;
        } 
        else
        {
            rbInactive.IsChecked = true;
        }
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
        var user = (DataGridUser.SelectedItem as UserTable)?.UserId;
        using DBContext db = new DBContext();
        var userToEdit = db.UserTables.FirstOrDefault(u => u.UserId == user);
        if (userToEdit != null)
        {
            userToEdit.FullName = txtFullName.Text;
            userToEdit.UserName = txtUsername.Text;
            userToEdit.RoleId = db.RoleTables.FirstOrDefault(r => r.RoleName == cmbRole.Text)?.RoleId ?? 1;
            userToEdit.Email = txtEmail.Text;
            if (rbActive.IsChecked == true)
            {
                userToEdit.Status = true;
            }
            else
            {
                userToEdit.Status = false;
            }
        }
        db.SaveChanges();
        GetAllUser();
    }

    private void BtnBan_OnClick(object sender, RoutedEventArgs e)
    {
        var user = (DataGridUser.SelectedItem as UserTable)?.UserId;
        using DBContext db = new DBContext();
        var userToBan = db.UserTables.FirstOrDefault(u => u.UserId == user);
        if (userToBan != null) userToBan.Status = false;
        db.SaveChanges();
        GetAllUser();
    }

    private void BtnDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var user = (DataGridUser.SelectedItem as UserTable)?.UserId;
        using DBContext db = new DBContext();
        var userToDelete = db.UserTables.FirstOrDefault(u => u.UserId == user);
        if (userToDelete != null) db.UserTables.Remove(userToDelete);
        db.SaveChanges();
        GetAllUser();
    }
}