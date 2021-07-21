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

            window.Width = Math.Max(204 * Scale, 408);
            window.Height = Math.Max(51 * Scale, 102);
        }
    }
}
