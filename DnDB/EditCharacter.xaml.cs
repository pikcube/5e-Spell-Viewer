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

namespace DnDB
{
    /// <summary>
    /// Interaction logic for EditCharacter.xaml
    /// </summary>
    public partial class EditCharacter : Window
    {
        public EditCharacter()
        {
            InitializeComponent();

            FontSize = 8 * MainWindow.SettingsVariables.Scale;
            CreateChara.FontSize = 10 * MainWindow.SettingsVariables.Scale;
            RenameChara.FontSize = 10 * MainWindow.SettingsVariables.Scale;
            DeleteChara.FontSize = 10 * MainWindow.SettingsVariables.Scale;

            FontFamily = MainWindow.SettingsVariables.SelectedFont;
            CreateChara.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            RenameChara.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            DeleteChara.FontFamily = MainWindow.SettingsVariables.SelectedFont;

            Foreground = MainWindow.SettingsVariables.TextColor;
            CreateChara.Foreground = MainWindow.SettingsVariables.TextColor;
            RenameChara.Foreground = MainWindow.SettingsVariables.TextColor;
            DeleteChara.Foreground = MainWindow.SettingsVariables.TextColor;

            Background = MainWindow.SettingsVariables.BackgroundColor;
            CreateChara.Background = MainWindow.SettingsVariables.ButtonBrush;
            RenameChara.Background = MainWindow.SettingsVariables.ButtonBrush;
            DeleteChara.Background = MainWindow.SettingsVariables.ButtonBrush;


        }

        private void CreateChara_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.EditResult = 0;
            Close();
        }

        private void RenameChara_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.EditResult = 1;
            Close();

        }

        private void DeleteChara_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.EditResult = 2;
            Close();
        }
    }
}
