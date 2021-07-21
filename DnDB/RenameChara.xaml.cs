using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private static double Scale => MainWindow.SettingsVariables.Scale;

        private void UpdateTextSize()
        {
            CharacterLabel.FontSize = 8 * Scale;
            NewNameLabel.FontSize = 8 * Scale;
            CharacterComboBox.FontSize = 8 * Scale;
            NewNameTextBox.FontSize = 8 * Scale;
            RenameButton.FontSize = 8 * Scale;

            CharacterLabel.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            NewNameLabel.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            CharacterComboBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            NewNameTextBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            RenameButton.FontFamily = MainWindow.SettingsVariables.SelectedFont;

            window.Width = Math.Max(210 * Scale, 420);
            window.Height = Math.Max(95 * Scale, 190);
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            try
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

                if (MainWindow.IsReservedFileName(NewNameTextBox.Text.ToLower()))
                {
                    Process.Start(@"https://www.youtube.com/watch?v=bC6tngl0PTI");
                    MessageBox.Show($"You can't name a file {NewNameTextBox.Text} in Windows");
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
            catch (Exception ex)
            {
                File.WriteAllText("error.txt", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show("Something went wrong, check the error file");
            }
        }
    }
}
