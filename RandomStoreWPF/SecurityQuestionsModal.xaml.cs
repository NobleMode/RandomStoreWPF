using System.Windows;
using System.Windows.Controls;
using RandomStoreWPF.Models;

namespace RandomStoreWPF;

public partial class SecurityQuestionsModal : Window
{
    private List<SecurityQuestion> _questions;
    public List<SecurityAnswer> UserSecurityQuestions { get; set; }
    
    public SecurityQuestionsModal()
    {
        InitializeComponent();
        GetQuestions();
        SetQuestionsToComboBoxes();
    }
    
    private void GetQuestions()
    {
        using DBContext db = new DBContext();
        _questions = db.SecurityQuestions.ToList();
    }

    private void SetQuestionsToComboBoxes()
    {
        Question1ComboBox.ItemsSource = _questions.ToList();
        Question2ComboBox.ItemsSource = _questions.ToList();
        Question3ComboBox.ItemsSource = _questions.ToList();

        Question1ComboBox.SelectionChanged += QuestionComboBox_SelectionChanged;
        Question2ComboBox.SelectionChanged += QuestionComboBox_SelectionChanged;
        Question3ComboBox.SelectionChanged += QuestionComboBox_SelectionChanged;
    }
    
    private int _complexityCounter = 0; // Step 1: Initialize complexity counter
    
    private void IncrementComplexity(SecurityQuestion question)
    {
        _complexityCounter += question.Severity ?? 1;
        TbxComplex.Text = $"{_complexityCounter} / 6";
    }

    private void QuestionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!(sender is ComboBox comboBox)) return;

        var selectedQuestion = comboBox.SelectedItem as SecurityQuestion;
        if (selectedQuestion == null) return;

        // Reset complexity counter on each selection change to recalculate based on current selections
        _complexityCounter = 0;
        var selectedQuestions = new List<SecurityQuestion>
        {
            Question1ComboBox.SelectedItem as SecurityQuestion,
            Question2ComboBox.SelectedItem as SecurityQuestion,
            Question3ComboBox.SelectedItem as SecurityQuestion
        }.Where(q => q != null).ToList();

        // Recalculate complexity based on all selected questions
        foreach (var question in selectedQuestions)
        {
            IncrementComplexity(question);
        }

        UpdateComboBoxItems(Question1ComboBox, selectedQuestions, comboBox);
        UpdateComboBoxItems(Question2ComboBox, selectedQuestions, comboBox);
        UpdateComboBoxItems(Question3ComboBox, selectedQuestions, comboBox);
    }

    private void UpdateComboBoxItems(ComboBox comboBox, List<SecurityQuestion> excludedQuestions, ComboBox sourceComboBox)
    {
        if (comboBox == sourceComboBox) return;

        var currentSelection = comboBox.SelectedItem;
        comboBox.ItemsSource = _questions.Where(q => !excludedQuestions.Contains(q) || q == currentSelection).ToList();
        comboBox.SelectedItem = currentSelection;
    }
    
    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (Answer1TextBox.Text == "" || Answer2TextBox.Text == "" || Answer3TextBox.Text == "")
        {
            ErrorTextBlock.Text = "Please fill in all the answers.";
            return;
        }
        
        if (_complexityCounter < 6)
        {
            ErrorTextBlock.Text = "Please select more complex questions.";
            return;
        }
        
        UserSecurityQuestions = new List<SecurityAnswer>
        {
            new() {q = Question1ComboBox.SelectedItem as SecurityQuestion, a = Answer1TextBox.Text},
            new() {q = Question2ComboBox.SelectedItem as SecurityQuestion, a = Answer2TextBox.Text},
            new() {q = Question3ComboBox.SelectedItem as SecurityQuestion, a = Answer3TextBox.Text}
        };
        
        this.DialogResult = true;
    }
}

public class SecurityAnswer
{
    public SecurityQuestion q { get; set; }
    public string a { get; set; }
}