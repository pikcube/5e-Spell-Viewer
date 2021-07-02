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
using Path = System.Windows.Shapes.Path;

namespace DnDB
{
    /// <summary>
    /// Interaction logic for RenameChara.xaml
    /// </summary>
    public partial class RenameChara : Window
    {
        public RenameChara()
        {
            InitializeComponent();
            CharacterComboBox.ItemsSource= MainWindow.Characters.Select(z => z.ClassName).ToList();
            UpdateTextSize();
        }

        private double Scale => MainWindow.SettingsVariables.Scale;

        private void UpdateTextSize()
        {
            CharacterLabel.FontSize = 8 * Scale;
            NewNameLabel.FontSize = 8 * Scale;
            CharacterComboBox.FontSize = 8 * Scale;
            NewNameTextBox.FontSize = 8 * Scale;
            RenameButton.FontSize = 8 * Scale;

            window.Width = Math.Max(210 * Scale, 420);
            window.Height = Math.Max(95 * Scale, 190);
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            string[] Existing = Directory.GetFiles("classes");
            if (Existing.Any(z => z == $@"classes\{NewNameTextBox.Text}.dndbClass") || Existing.Any(z => z == $@"classes\{NewNameTextBox.Text}.dndbChara"))
            {
                MessageBox.Show("Character/Class Already Exists");
                return;
            }

            if (NewNameTextBox.Text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
            {
                MessageBox.Show($"Names can't contain {string.Join("", System.IO.Path.GetInvalidFileNameChars())}");
                return;
            }

            if (NewNameTextBox.Text == "")
            {
                MessageBox.Show("Enter a new character name");
                return;
            }

            string[] a = File.ReadAllLines($@"classes\{CharacterComboBox.Text}.dndbChara");
            File.WriteAllLines($@"classes\{NewNameTextBox.Text}.dndbChara", a);
            File.Delete($@"classes\{CharacterComboBox.Text}.dndbChara");

            MainWindow.NewClass = NewNameTextBox.Text;
            Close();
        }
    }
}
