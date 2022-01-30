using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace DnDB
{
    /// <summary>
    /// Interaction logic for DeleteChara.xaml
    /// </summary>
    public partial class DeleteChara : Window
    {
        public DeleteChara()
        {
            InitializeComponent();
            CharacterComboBox.ItemsSource = MainWindow.Characters.Select(z => z.ClassName).ToList();
            UpdateTextSize();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete($@"classes\{CharacterComboBox.Text}.dndbChara");
            if (File.Exists($@"classes\{CharacterComboBox.Text}.dndbChara.prep"))
            {
                File.Delete($@"classes\{CharacterComboBox.Text}.dndbChara.prep");
            }
            Close();
        }

        private double Scale => MainWindow.SettingsVariables.Scale;

        private void UpdateTextSize()
        {
            CharacterLabel.FontSize = 8 * Scale;
            CharacterComboBox.FontSize = 8 * Scale;
            DeleteButton.FontSize = 8 * Scale;

            CharacterLabel.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            CharacterComboBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            DeleteButton.FontFamily = MainWindow.SettingsVariables.SelectedFont;

            CharacterLabel.Foreground = MainWindow.SettingsVariables.TextColor;
            CharacterComboBox.Foreground = MainWindow.SettingsVariables.TextColor;
            DeleteButton.Foreground = MainWindow.SettingsVariables.TextColor;

            Background = MainWindow.SettingsVariables.BackgroundColor;
            CharacterComboBox.Style = MainWindow.SettingsVariables.ComboStyle;
            DeleteButton.Background = MainWindow.SettingsVariables.ButtonBrush;

            window.Width = Math.Max(204 * Scale, 408);
            window.Height = Math.Max(51 * Scale, 102);
        }
    }
}
