using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Drawing.Text;

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

        bool DarkModeEnabled
        {
            get
            {
                return MainWindow.SettingsVariables.DarkModeEnabled;
            }
            set
            {
                MainWindow.SettingsVariables.DarkModeEnabled = value;
            }
        }

        public Options()
        {
            InitializeComponent();
            string[] comboBoxStrings = { "Tiny", "Smaller", "Medium (Default)", "Bigger", "Large", "Huge", "Maximum", };
            string[] DarkStrings = { "Light Mode", "Dark Mode" };
            TextSizeBox.ItemsSource = comboBoxStrings;
            DarkBox.ItemsSource = DarkStrings;
            TextSizeBox.SelectedIndex = (int)(2 * GlobalSize - 2);
            LevelCheckBox.IsChecked = MainWindow.SettingsVariables.SortByLevel;
            SearchCheckBox.IsChecked = MainWindow.SettingsVariables.ShowSearchBox;
            VSMCheckBox.IsChecked = MainWindow.SettingsVariables.UseFullVSM;
            DarkBox.SelectedIndex = DarkModeEnabled ? 1 : 0;
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
            DarkBox.FontSize = 8 * GlobalSize;
            DarkLabel.FontSize = 8 * GlobalSize;

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
            DarkBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            DarkLabel.FontFamily = MainWindow.SettingsVariables.SelectedFont;

            WindowObject.Foreground = MainWindow.SettingsVariables.TextColor;
            AdjustLabel.Foreground = MainWindow.SettingsVariables.TextColor;
            ModifyLabel.Foreground = MainWindow.SettingsVariables.TextColor;
            TextSizeBox.Foreground = MainWindow.SettingsVariables.TextColor;
            OpenClassButton.Foreground = MainWindow.SettingsVariables.TextColor;
            LevelCheckBox.Foreground = MainWindow.SettingsVariables.TextColor;
            SearchCheckBox.Foreground = MainWindow.SettingsVariables.TextColor;
            VSMCheckBox.Foreground = MainWindow.SettingsVariables.TextColor;
            FontBox.Foreground = MainWindow.SettingsVariables.TextColor;
            FontBox.Foreground = MainWindow.SettingsVariables.TextColor;
            FontLabel.Foreground = MainWindow.SettingsVariables.TextColor;
            SpellsLabel.Foreground = MainWindow.SettingsVariables.TextColor;
            OpenSpellsButton.Foreground= MainWindow.SettingsVariables.TextColor;
            DarkBox.Foreground= MainWindow.SettingsVariables.TextColor;
            DarkLabel.Foreground = MainWindow.SettingsVariables.TextColor;

            WindowObject.Background = MainWindow.SettingsVariables.BackgroundColor;
            AdjustLabel.Background = MainWindow.SettingsVariables.BackgroundColor;
            ModifyLabel.Background = MainWindow.SettingsVariables.BackgroundColor;
            TextSizeBox.Style = MainWindow.SettingsVariables.ComboStyle;
            OpenClassButton.Background = MainWindow.SettingsVariables.ButtonBrush;
            //LevelCheckBox.Background = MainWindow.SettingsVariables.BackgroundColor;
            //SearchCheckBox.Background = MainWindow.SettingsVariables.BackgroundColor;
            //VSMCheckBox.Background = MainWindow.SettingsVariables.BackgroundColor;
            FontBox.Style = MainWindow.SettingsVariables.ComboStyle;
            FontLabel.Background = MainWindow.SettingsVariables.BackgroundColor;
            SpellsLabel.Background = MainWindow.SettingsVariables.BackgroundColor;
            OpenSpellsButton.Background = MainWindow.SettingsVariables.ButtonBrush;
            DarkBox.Style = MainWindow.SettingsVariables.ComboStyle;
            DarkLabel.Background = MainWindow.SettingsVariables.BackgroundColor;
            


            WindowObject.Width = Math.Max(201 * GlobalSize, 402);
            WindowObject.Height = Math.Max(201 * GlobalSize, 402);

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalSize = (double)(TextSizeBox.SelectedIndex + 2) / 2;
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

        private void DarkBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DarkModeEnabled = DarkBox.SelectedIndex != 0;
            UpdateTextSize();
        }
    }
}
