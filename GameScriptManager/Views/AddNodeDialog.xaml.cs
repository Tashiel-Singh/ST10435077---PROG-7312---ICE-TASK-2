using System.Windows;

namespace GameScriptManager.Views;

public partial class AddNodeDialog : Window
{
    public int StoryNumber { get; private set; }
    public string StoryText { get; private set; } = string.Empty;
    public bool Prepend { get; private set; }

    public AddNodeDialog(int suggestedNumber = 1)
    {
        InitializeComponent();
        TxtStoryNumber.Text = suggestedNumber.ToString();
        TxtStoryText.Focus();
        TxtStoryText.SelectAll();
    }

    private void OnAddClick(object sender, RoutedEventArgs e)
    {
        TxtNumberError.Visibility = Visibility.Collapsed;

        if (!int.TryParse(TxtStoryNumber.Text.Trim(), out int num) || num <= 0)
        {
            TxtNumberError.Text = "Please enter a valid positive integer story number.";
            TxtNumberError.Visibility = Visibility.Visible;
            TxtStoryNumber.Focus();
            return;
        }

        string text = TxtStoryText.Text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            TxtNumberError.Text = "Story paragraph text cannot be empty.";
            TxtNumberError.Visibility = Visibility.Visible;
            TxtStoryText.Focus();
            return;
        }

        StoryNumber = num;
        StoryText = text;
        Prepend = RbPrepend.IsChecked == true;

        DialogResult = true;
        Close();
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
