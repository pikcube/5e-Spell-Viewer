using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
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
using System.Drawing.Text;
using FontFamily = System.Drawing.FontFamily;

namespace DnDB
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options
    {
        double GlobalSize
        {
            get
            {
                return MainWindow.Scale;
            }
            set
            {
                MainWindow.Scale = value;
            }
        }
        
        public Options()
        {
            InitializeComponent();
            string[] comboBoxStrings = {"Tiny", "Smaller", "Medium (Default)", "Bigger", "Large", "Huge", "Maximum",};
            TextSizeBox.ItemsSource = comboBoxStrings;
            TextSizeBox.SelectedIndex = (int) (2 * GlobalSize - 2);
            LevelCheckBox.IsChecked = MainWindow.SettingsVariables.SortByLevel;
            SearchCheckBox.IsChecked = MainWindow.SettingsVariables.ShowSearchBox;
            VSMCheckBox.IsChecked = MainWindow.SettingsVariables.UseFullVSM;
            List<string> fontBoxString = new List<string>();
            using (InstalledFontCollection C = new InstalledFontCollection())
            {
                fontBoxString.AddRange(C.Families.Select(F => F.Name));
            }
            FontBox.ItemsSource = fontBoxString.OrderBy(z => z).ToList();
            FontBox.SelectedItem = MainWindow.SettingsVariables.SelectedFont.ToString();
            
            UpdateTextSize();
        }

        private void UpdateTextSize()
        {
            FontBox.FontSize = 8 * GlobalSize;
            FontLabel.FontSize = 8 * GlobalSize;
            WindowObject.FontSize = 6 * GlobalSize;
            AdjustLabel.FontSize = 8 * GlobalSize;
            ModifyLabel.FontSize = 8 * GlobalSize;
            TextSizeBox.FontSize = 8 * GlobalSize;
            OpenClassButton.FontSize = 8 * GlobalSize;
            LevelCheckBox.FontSize = 8 * GlobalSize;
            SearchCheckBox.FontSize = 8 * GlobalSize;
            VSMCheckBox.FontSize = 8 * GlobalSize;
            SpellsLabel.FontSize = 8 * GlobalSize;
            OpenSpellsButton.FontSize = 8 * GlobalSize;

            WindowObject.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            AdjustLabel.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            ModifyLabel.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            TextSizeBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            OpenClassButton.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            LevelCheckBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            SearchCheckBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            VSMCheckBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            FontBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            FontLabel.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            SpellsLabel.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            OpenSpellsButton.FontFamily = MainWindow.SettingsVariables.SelectedFont;

            WindowObject.Width = Math.Max(201 * GlobalSize, 402);
            WindowObject.Height = Math.Max(181 * GlobalSize, 362);

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalSize = (double) (TextSizeBox.SelectedIndex + 2) / 2;
            UpdateTextSize();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo a = new ProcessStartInfo
            {
                Arguments = $"\"{Directory.GetCurrentDirectory()}\\classes\"",
                FileName = "explorer.exe",
            };
            Process.Start(a);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (VSMCheckBox.IsChecked != null)
            {
                MainWindow.SettingsVariables.UseFullVSM = VSMCheckBox.IsChecked.Value;
            }
        }

        private void SearchCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (SearchCheckBox.IsChecked != null)
            {
                MainWindow.SettingsVariables.ShowSearchBox = SearchCheckBox.IsChecked.Value;
            }
        }

        private void LevelCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (LevelCheckBox.IsChecked != null)
            {
                MainWindow.SettingsVariables.SortByLevel = LevelCheckBox.IsChecked.Value;
            }
        }

        private void FontBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object fontBoxSelectedItem = FontBox.SelectedItem;
            if (fontBoxSelectedItem == null)
            {
                return;
            }
            MainWindow.SettingsVariables.SetSelectedFont(fontBoxSelectedItem.ToString());
            UpdateTextSize();
        }

        private void OpenSpellsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("spells.custom.json");
            }
            catch (Exception)
            {
                MainWindow.CreateCustomJson();
                try
                {
                    Process.Start("spells.custom.json");
                }
                catch (Exception)
                {
                    ProcessStartInfo a = new ProcessStartInfo
                    {
                        Arguments = $"\"{Directory.GetCurrentDirectory()}\"",
                        FileName = "explorer.exe",
                    };
                    Process.Start(a);
                }
            }

            try
            {
                Process.Start("README.example.spells.custom.json");
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
