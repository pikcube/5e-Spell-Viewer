﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace DnDB
{
    /// <summary>
    /// Interaction logic for CreateChara.xaml
    /// </summary>
    public partial class CreateChara
    {
        public CreateChara()
        {
            InitializeComponent();
            List<string> comboBox1DataSource = MainWindow.Classes.Select(z => z.ClassName).ToList();
            comboBox1DataSource.RemoveAt(0);
            ClassComboBox.ItemsSource = comboBox1DataSource;
            UpdateTextSize();
        }

        private static double Scale => MainWindow.SettingsVariables.Scale;

        private void UpdateTextSize()
        {
            ClassLabel.FontSize = 8 * Scale;
            NameLabel.FontSize = 8 * Scale;
            ImportCheckBox.FontSize = 8 * Scale;
            NameTextBox.FontSize = 8 * Scale;
            ClassComboBox.FontSize = 8 * Scale;
            CreateButton.FontSize = 8 * Scale;

            ClassLabel.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            NameLabel.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            ImportCheckBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            NameTextBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            ClassComboBox.FontFamily = MainWindow.SettingsVariables.SelectedFont;
            CreateButton.FontFamily = MainWindow.SettingsVariables.SelectedFont;

            ClassLabel.Foreground = MainWindow.SettingsVariables.TextColor;
            NameLabel.Foreground = MainWindow.SettingsVariables.TextColor;
            ImportCheckBox.Foreground = MainWindow.SettingsVariables.TextColor;
            NameTextBox.Foreground = MainWindow.SettingsVariables.TextColor;
            ClassComboBox.Foreground = MainWindow.SettingsVariables.TextColor;
            CreateButton.Foreground = MainWindow.SettingsVariables.TextColor;

            Background = MainWindow.SettingsVariables.BackgroundColor;
            NameTextBox.Background = MainWindow.SettingsVariables.BackgroundColor;
            ClassComboBox.Style = MainWindow.SettingsVariables.ComboStyle;
            CreateButton.Background = MainWindow.SettingsVariables.ButtonBrush;


            window.Width = Math.Max(230 * Scale, 460);
            window.Height = Math.Max(90 * Scale, 180);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
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

                if (MainWindow.IsReservedFileName(NameTextBox.Text.ToLower()))
                {
                    Process.Start(@"https://www.youtube.com/watch?v=bC6tngl0PTI");
                    MessageBox.Show($"You can't name a file {NameTextBox.Text} in Windows");
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
            catch (Exception ex)
            {
                File.WriteAllText("error.txt", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show("Something went wrong, check the error file");
            }
        }
    }
}
