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
    /// Interaction logic for CreateChara.xaml
    /// </summary>
    public partial class CreateChara : Window
    {
        public CreateChara()
        {
            InitializeComponent();
            List<string> comboBox1DataSource = MainWindow.Classes.Select(z => z.ClassName).ToList();
            comboBox1DataSource.RemoveAt(0);
            ClassComboBox.ItemsSource= comboBox1DataSource;
            UpdateTextSize();
        }

        private double Scale => MainWindow.SettingsVariables.Scale;

        private void UpdateTextSize()
        {
            ClassLabel.FontSize = 8 * Scale;
            NameLabel.FontSize = 8 * Scale;
            ImportCheckBox.FontSize = 8 * Scale;
            NameTextBox.FontSize = 8 * Scale;
            ClassComboBox.FontSize = 8 * Scale;
            CreateButton.FontSize = 8 * Scale;

            window.Width *= Scale / 2;
            window.Height *= Scale / 2;

            window.Width = Math.Max(230 * Scale, 460);
            window.Height = Math.Max(90 * Scale, 180);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string[] Existing = Directory.GetFiles("classes");
            if (Existing.Any(z => z == $@"classes\{NameTextBox.Text}.dndbClass") || Existing.Any(z => z == $@"classes\{NameTextBox.Text}.dndbChara"))
            {
                MessageBox.Show("Character/Class Already Exists");
                return;
            }

            if (NameTextBox.Text == "")
            {
                MessageBox.Show("Enter a character name");
                return;
            }

            using (var writer = new StreamWriter($@"classes\{NameTextBox.Text}.dndbChara", false))
            {
                writer.Write("");
            }

            if (ImportCheckBox.IsChecked != null && ImportCheckBox.IsChecked.Value)
            {
                if (File.Exists($@"classes\{ClassComboBox.Text}.dndbClass"))
                {
                    File.AppendAllLines($@"classes\{NameTextBox.Text}.dndbChara", File.ReadAllLines($@"classes\{ClassComboBox.Text}.dndbClass"));
                }

                if (File.Exists($@"classes\{ClassComboBox.Text}.dndbChara"))
                {
                    File.AppendAllLines($@"classes\{NameTextBox.Text}.dndbChara", File.ReadAllLines($@"classes\{ClassComboBox.Text}.dndbChara"));
                }
            }

            MainWindow.NewClass = NameTextBox.Text;
            Close();
        }
    }
}
