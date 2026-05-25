using System.Windows;
using System.Windows.Input;
using AppleCIDR.ViewModels;

namespace AppleCIDR;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void ThemeToggle_Changed(object sender, RoutedEventArgs e)
    {
        bool useLightTheme = ThemeToggle.IsChecked == true;
        string themePath = useLightTheme ? "Themes/LightTheme.xaml" : "Themes/DarkTheme.xaml";
        ThemeLabel.Text = useLightTheme ? "Light" : "Dark";

        ResourceDictionary newTheme = new()
        {
            Source = new Uri(themePath, UriKind.Relative)
        };

        ResourceDictionaryCollection dictionaries = Application.Current.Resources.MergedDictionaries;
        ResourceDictionary? currentTheme = dictionaries.FirstOrDefault(dictionary =>
            dictionary.Source is not null &&
            (dictionary.Source.OriginalString.EndsWith("DarkTheme.xaml", StringComparison.OrdinalIgnoreCase) ||
             dictionary.Source.OriginalString.EndsWith("LightTheme.xaml", StringComparison.OrdinalIgnoreCase)));

        if (currentTheme is not null)
        {
            int index = dictionaries.IndexOf(currentTheme);
            dictionaries[index] = newTheme;
        }
        else
        {
            dictionaries.Insert(0, newTheme);
        }
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
