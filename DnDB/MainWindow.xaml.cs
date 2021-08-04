using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Newtonsoft.Json;

namespace DnDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class OptionsVariables
    {
        private readonly string path;

        public OptionsVariables(string p)
        {
            try
            {
                path = p;
                if (!File.Exists(path))
                {
                    MakeDefaultConfigFile();
                }

                Contents = File.ReadAllLines(path);

                if (Contents.Length == 5)
                {
                    return;
                }

                MakeDefaultConfigFile();
            }
            catch
            {
                MakeDefaultConfigFile();
            }
            finally
            {
                Contents = File.ReadAllLines(path);
            }
        }

        private void MakeDefaultConfigFile()
        {
            string[] a =
            {
                2.0.ToString(),
                false.ToString(),
                true.ToString(),
                true.ToString(),
                "Calibri",
                //false.ToString(),
            };
            File.WriteAllLines(path, a);
        }

        private readonly string[] Contents;


        private void SaveOptionsConfig() => File.WriteAllLines(path, Contents);

        public double Scale
        {
            get => double.Parse(Contents[0]);
            set
            {
                Contents[0] = value.ToString();
                SaveOptionsConfig();
            }
        }

        public bool UseFullVSM
        {
            get => bool.Parse(Contents[1]);
            set
            {
                Contents[1] = value.ToString();
                SaveOptionsConfig();
            }
        }

        public bool ShowSearchBox
        {
            get => bool.Parse(Contents[2]);
            set
            {
                Contents[2] = value.ToString();
                SaveOptionsConfig();
            }
        }

        public bool SortByLevel
        {
            get => bool.Parse(Contents[3]);
            set
            {
                Contents[3] = value.ToString();
                SaveOptionsConfig();
            }
        }

        public FontFamily SelectedFont
        {
            get => new FontFamily(Contents[4]);
        }

        public void SetSelectedFont(string FontName)
        {
            try
            {
                _ = new FontFamily(FontName);
                Contents[4] = FontName;
                SaveOptionsConfig();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //public bool DarkModeEnabled
        //{
        //    get => true;
        //    //get => bool.Parse(Contents[5]);
        //    set
        //    {
        //        Contents[5] = value.ToString();
        //        SaveOptionsConfig();
        //    }
        //}

        //public Brush TextColor
        //{
        //    get => DarkModeEnabled ? Brushes.White : Brushes.Black;
        //}

        //public Brush BackgroundColor
        //{
        //    get => DarkModeEnabled ? Brushes.Black : Brushes.White;
        //}

        //public LinearGradientBrush BackgroundGradientBrush => DarkModeEnabled
        //    ? new LinearGradientBrush(GradientStopCollection.Parse("#00000000,0 #00000000,1"))
        //    : new LinearGradientBrush(GradientStopCollection.Parse("#FFF0F0F0,0 #FFE5E5E5,1"));

        //public Brush ButtonBrush
        //{
        //    get => DarkModeEnabled ? new SolidColorBrush(Color.FromArgb(255, 34, 34, 34)) : new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
        //}

    }
    
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static OptionsVariables SettingsVariables;

        public static List<DnDBClass> Classes;
        public static List<DnDBClass> Characters;
        //private static DnDBDataSetTableAdapters.Master_SpellsTableAdapter TableAdapter;
        private static List<MasterSpell> SpellTable;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Classes = new List<DnDBClass>();
                Characters = new List<DnDBClass>();
                try
                {
                    Directory.SetCurrentDirectory(
                        $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/DnDB_Beta");
                }
                catch (IOException)
                {
                    Directory.CreateDirectory(
                        $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/DnDB_Beta");
                    Directory.SetCurrentDirectory(
                        $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/DnDB_Beta");
                }
                SettingsVariables = new OptionsVariables($"{Directory.GetCurrentDirectory()}/main.config");

                string[] LevelList =
                {
                    "All Levels", "Cantrip", "Level 1", "Level 2", "Level 3", "Level 4",
                    "Level 5", "Level 6", "Level 7", "Level 8", "Level 9",
                };

                string[] Schools =
                {
                    "All Schools", "Abjuration", "Conjuration", "Divination", "Enchantment",
                    "Evocation", "Illusion", "Necromancy", "Transmutation",
                };


                SelectedLevel.ItemsSource = LevelList;
                SelectedLevel.SelectedIndex = 0;
                SelectedLevel.SelectionChanged += SelectedClass_SelectionChanged;

                SelectedSchool.ItemsSource = Schools;
                SelectedSchool.SelectedIndex = 0;
                SelectedSchool.SelectionChanged += SelectedClass_SelectionChanged;

                var I = JsonConvert.DeserializeObject<Root>(File.ReadAllText("spells.json"));
                SpellTable = I.MasterSpells;
                SpellDescription.TextWrapping = TextWrapping.Wrap;


                UpdateClasses();
                UpdateFontSize();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                MessageBox.Show(exception.StackTrace);
            }
        }

        public static bool IsReservedFileName(string FileName)
        { 
            string[] ReservedNames =
            {
                "con",
                "prn",
                "aux",
                "nul",
                "com1",
                "com2",
                "com3",
                "com4",
                "com5",
                "com6",
                "com7",
                "com8",
                "com9",
                "lpt1",
                "lpt2",
                "lpt3",
                "lpt4",
                "lpt5",
                "lpt6",
                "lpt7",
                "lpt8",
                "lpt9",
                "clock$",
            };
            return ReservedNames.Any(z => z == FileName);
        }

        private void SelectedClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSpellListContents();
        }

        private void UpdateSpellListContents()
        {
            IEnumerable<SpellRow> SpellsToFilter;
            if (SelectedClass.SelectedIndex < 0 || SelectedClass.SelectedIndex >= SelectedClass.Items.Count)
            {
                SelectedClass.SelectedIndex = 0;
            }
            
            try
            {
                SpellsToFilter = SpellTable.Select(z => SpellRow.GetSpell(z.Name)).Where(z =>
                    Classes[SelectedClass.SelectedIndex].Spells.Any(x => x.Name == z.Name) &&
                    (z.Level + 1 == SelectedLevel.SelectedIndex || SelectedLevel.SelectedIndex == 0) &&
                    (z.School == SelectedSchool.SelectedItem.ToString() || SelectedSchool.SelectedIndex == 0));
            }
            catch(Exception)
            {
                RestoreDefaultClassFiles();
                return;
            }

            if (SearchBox.Text != string.Empty)
            {
                SpellsToFilter = SpellsToFilter.Where(z => z.Name.ToLower().Contains(SearchBox.Text.ToLower()));
            }

            //Apparently you can't update a member of an IEnumberable from a foreach loop, it just updates a local copy and throws it away.
            List<SpellRow> SpellsFiltered = SpellsToFilter.ToList();
            
            foreach (SpellRow s in SpellsFiltered.Where(s => IsPreparedSpellInCurrentClass(s.Name)))
            {
                s.Name = $"*{s.Name}";
            }

            if (SettingsVariables.SortByLevel)
            {
                SpellList.ItemsSource = SpellsFiltered
                    .OrderBy(z => z.Name[0] != '*') //False shows up first before true, so this check is inverted. Given 1 = true, makes sense
                    .ThenBy(z => z.Level)
                    .ThenBy(z => z.Name).Select(z => z.Name);
            }
            else
            {
                SpellList.ItemsSource = SpellsFiltered
                    .OrderBy(z => z.Name[0] != '*') //False shows up first before true, so this check is inverted. Given 1 = true, makes sense
                    .ThenBy(z => z.Name).Select(z => z.Name);
            }
            SpellList.SelectedIndex = 0;
        }

        private void RestoreDefaultClassFiles()
        {
            MessageBox.Show(
                "Something went wrong reading a class file, we will attempt to restore class files back to default and close the app");
            
            string[] ExistingClasses = Directory.GetFiles("classes", "*.dndbClass");
            string[] DefaultClasses = Directory.GetFiles("classesDefault", "*.dndbClass");
            
            foreach (string p in ExistingClasses)
            {
                File.Delete(p);
            }

            foreach (string p in DefaultClasses)
            {
                File.Copy(p, p.Replace("Default", ""));
            }

            Close();
        }

        private void UpdateClasses()
        {
            int SidebarIndex = SpellList.SelectedIndex;

            string[] ClassFiles = Directory.GetFiles("classes", "*.dndbClass");
            string[] CharacterFiles = Directory.GetFiles("classes", "*.dndbChara");

            Classes.Clear();
            Characters.Clear();

            foreach (string c in ClassFiles)
            {
                Classes.Add(DnDBClass.GetClass(c));
            }

            foreach (string c in CharacterFiles)
            {
                Classes.Add(DnDBClass.GetClass(c));
                Characters.Add(DnDBClass.GetClass(c));
            }

            int OldIndex1 = SelectedClass.SelectedIndex == -1 ? 0 : SelectedClass.SelectedIndex;
            OldIndex1 = SelectedClass.SelectedIndex >= SelectedClass.Items.Count
                ? AddToThisClass.Items.Count - 1
                : OldIndex1;
            int OldIndex2 = AddToThisClass.SelectedIndex == -1 ? 0 : AddToThisClass.SelectedIndex;
            OldIndex2 = AddToThisClass.SelectedIndex >= AddToThisClass.Items.Count
                ? AddToThisClass.Items.Count - 1
                : OldIndex2;

            SelectedClass.ItemsSource = Classes.Select(z => z.ClassName);

            SelectedClass.SelectedIndex = 0;

            SelectedClass.SelectedIndex = OldIndex1;




            AddToThisClass.ItemsSource = Characters.Select(z => z.ClassName);

            if (AddToThisClass.Items.Count == 0)
            {
                AddToThisClass.IsEnabled = false;
                RenameChara.IsEnabled = false;
                DeleteChara.IsEnabled = false;
                AddSpell.IsEnabled = false;
                RemoveSpell.IsEnabled = false;
            }
            else
            {
                AddToThisClass.IsEnabled = true;
                RenameChara.IsEnabled = true;
                DeleteChara.IsEnabled = true;
                AddSpell.IsEnabled = true;
                RemoveSpell.IsEnabled = true;

                AddToThisClass.SelectedIndex = 0;

                AddToThisClass.SelectedIndex = OldIndex2;

                SpellList.SelectedIndex = Math.Min(SidebarIndex, SpellList.Items.Count - 1);
            }
            return;
        }

        private void SpellList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpellList.SelectedItem == null)
            {
                return;
            }

            RefreshSpellDisplay();
        }

        private void RefreshSpellDisplay()
        {
            SpellRow Spell = SpellRow.GetSpell((string) SpellList.SelectedItem);
            SpellName.Text = Spell.Name;
            SpellLevel.Text = $"Level {Spell.Level}";
            SpellSchool.Text = Spell.School;
            SpellCastTime.Text = $"Casting Time:{Environment.NewLine}{Spell.CastingTime}";
            SpellDuration.Text = $"Duration:{Environment.NewLine}{Spell.Duration}";
            SpellRange.Text = $"Range:{Environment.NewLine}{Spell.Range}";
            SpellInfo.Text = Spell.OtherInfo;
            SpellDescription.Text = Spell.Details;
        }

        //DnD Class Object
        public class DnDBClass
        {
            public string ClassName
            {
                get;
            }

            public List<SpellRow> Spells
            {
                get;
            }

            public static DnDBClass GetClass(string Path)
            {
                List<string> Spells = File.ReadAllLines(Path).ToList();
                return new DnDBClass(Path, Spells);
            }


            public DnDBClass(string path, List<string> spells)
            {
                bool RewriteNeeded = false;
                ClassName = Path.GetFileNameWithoutExtension(path);
                Spells = new List<SpellRow>();
                foreach (string S in spells.ToArray())
                {
                    try
                    {
                        Spells.Add(SpellRow.GetSpell(S));
                    }
                    catch (Exception)
                    {
                        if (Path.GetExtension(path) == ".dndbChara")
                        {
                            spells.RemoveAll(z => z == S);
                            RewriteNeeded = true;
                        }
                        else
                        {
                            throw new Exception("Class File Corrupted");
                        }
                    }
                }

                if (RewriteNeeded)
                {
                    File.WriteAllLines(path, spells.ToArray());
                }
            }
        }

        //Spell Object
        public class SpellRow
        {
            public string Name;
            public int Level;
            public string School;
            public string CastingTime;
            public string Duration;
            public string Range;
            public string Area;
            public bool IsRitual;
            public bool IsConcentration;
            public bool IsVerbal;
            public bool IsSomatic;
            public bool IsMaterial;
            public string Material;
            public string Details;

            public string OtherInfo
            {
                get
                {
                    string Output = "";
                    if (IsRitual)
                    {
                        Output += "Is a Ritual, ";
                    }
                    if (IsConcentration)
                    {
                        Output += "Requires Concentration, ";
                    }

                    if (Area != "")
                    {
                        Output += $"Area: {Area}, ";
                    }

                    if (Output != "")
                    {
                        Output = Output.TrimEnd(' ').TrimEnd(',');
                        Output += Environment.NewLine;
                    }

                    Output += $"Components: {VSM(IsVerbal, IsSomatic, IsMaterial)}";

                    if (IsMaterial)
                    {
                        Output += $", {Material}";
                    }

                    return Output;
                }
            }

            private static string VSM(bool V, bool S, bool M)
            {
                string Output = "";
                if (SettingsVariables.UseFullVSM)
                {
                    if (V)
                    {
                        Output += "Verbal, ";
                    }

                    if (S)
                    {
                        Output += "Somatic, ";
                    }

                    if (M)
                    {
                        Output += "Material, ";
                    }

                    Output = Output.TrimEnd().TrimEnd(',');
                }
                else
                {
                    if (V)
                    {
                        Output += "V";
                    }

                    if (S)
                    {
                        Output += "S";
                    }

                    if (M)
                    {
                        Output += "M";
                    }
                }

                return Output;
            }

            public static SpellRow GetSpell(string SpellName)
            {
                MasterSpell Row = SpellTable.First(z => string.Equals(z.Name, SpellName.Trim('*'), StringComparison.CurrentCultureIgnoreCase));

                SpellTable[SpellTable.IndexOf(Row)].Name = SpellName.Trim('*');

                return new SpellRow(
                    SpellName.Trim('*'),
                    Row.Level,
                    Row.School,
                    Row.CastingX0020Time,
                    Row.Duration,
                    Row.Range,
                    Row.Area,
                    Row.Ritual,
                    Row.Concentration,
                    Row.V,
                    Row.S,
                    Row.M,
                    Row.Material,
                    Row.Details);
            }



            private SpellRow(string name, string level, string school, string castingTime, string duration, string range, string area,
                string isRitual, string isConcentration, string isVerbal, string isSomatic, string isMaterial, string material, string details)
            {
                Name = name;
                Level = int.Parse(level);
                School = school;
                CastingTime = castingTime;
                Duration = duration;
                Range = range;
                Area = area == "n/a" ? "" : area;
                IsRitual = isRitual.Equals("1");
                IsConcentration = isConcentration.Equals("1");
                IsVerbal = isVerbal.Equals("1");
                IsSomatic = isSomatic.Equals("1");
                IsMaterial = isMaterial.Equals("1");
                Material = material == "none" ? "" : material;
                Details = details;
            }
        }

        private void AddSpell_Click(object sender, RoutedEventArgs e)
        {
            if (SpellList.Items.Count == 0)
            {
                return;
            }
            string SelectedSpell = (string)SpellList.SelectedItem;
            if (SpellList.SelectedIndex == -1)
            {
                if (!AttemptToInferSelectedSpell(ref SelectedSpell))
                {
                    return;
                }
            }
            string AddToThisClassSelectedName = (string)AddToThisClass.SelectedItem;
            using (StreamWriter writer = new StreamWriter($@"classes\{AddToThisClassSelectedName}.dndbChara", true))
            {
                writer.WriteLine(SelectedSpell.TrimStart('*'));
            }
            UpdateClasses();
            MessageBox.Show($"{SelectedSpell} added to {AddToThisClassSelectedName}", "Add complete");
        }

        private bool AttemptToInferSelectedSpell(ref string SelectedSpell)
        {
            IEnumerable<string> a = SpellList.ItemsSource as IEnumerable<string>;
            if (a == null)
            {
                return false;
            }

            foreach (string s in a)
            {
                if (s.Trim('*') == SpellName.Text)
                {
                    SelectedSpell = SpellName.Text;
                }
            }

            return SelectedSpell != string.Empty;
        }

        private void RemoveSpell_Click(object sender, RoutedEventArgs e)
        {
            string SelectedSpell = (string)SpellList.SelectedItem;
            if (SpellList.SelectedIndex == -1)
            {
                if (!AttemptToInferSelectedSpell(ref SelectedSpell))
                {
                    return;
                }
            }
            string AddToThisClassSelectedName = (string)AddToThisClass.SelectedItem;
            MessageBoxResult Result = MessageBox.Show($"Are you sure you want to remove {SelectedSpell} from {AddToThisClassSelectedName}?", "Caution", MessageBoxButton.YesNo);
            if (Result != MessageBoxResult.Yes)
            {
                return;
            }
            DnDBClass Class = Classes.First(z => z.ClassName == AddToThisClassSelectedName);
            Class.Spells.RemoveAll(z => z.Name == SelectedSpell);
            File.WriteAllLines($@"classes\{AddToThisClassSelectedName}.dndbChara", Class.Spells.Select(z => z.Name));
            UpdateClasses();
            MessageBox.Show($"{SelectedSpell} removed from {AddToThisClassSelectedName}", "Remove complete");
        }

        public static string NewClass = "";

        private void CreateChara_Click(object sender, RoutedEventArgs e)
        {
            //Form1 F = new Form1 { StartPosition = FormStartPosition.CenterParent };
            CreateChara F = new CreateChara {WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this,};
            F.ShowDialog();
            if (NewClass == "")
            {
                return;
            }

            string NewClassFlagless = NewClass;
            NewClass = "";

            AddToThisClass.SelectedIndex = 0;
            UpdateClasses();
            AddToThisClass.SelectedIndex = Characters.IndexOf(Characters.First(z => z.ClassName == NewClassFlagless));
        }

        private void RenameChara_Click(object sender, RoutedEventArgs e)
        {
            RenameChara F = new RenameChara {WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this,};
            F.ShowDialog();
            if (NewClass == "")
            {
                return;
            }
            AddToThisClass.SelectedIndex = 0;
            UpdateClasses();
            string NewClassFlagless = NewClass;
            NewClass = "";
            AddToThisClass.SelectedIndex = Characters.IndexOf(Characters.First(z => z.ClassName == NewClassFlagless));
        }

        private void DeleteChara_Click(object sender, RoutedEventArgs e)
        {
            DeleteChara F = new DeleteChara {WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this,};
            F.ShowDialog();
            UpdateClasses();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            Options O = new Options {WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, };
            O.ShowDialog();
            UpdateFontSize();
            UpdateClasses();
            UpdateSpellListContents();
            RefreshSpellDisplay();
        }

        private void SelectedLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UpdateFontSize()
        {
            SelectedClass.FontSize = 7 * Scale;
            SelectedLevel.FontSize = 7 * Scale;
            SelectedSchool.FontSize = 7 * Scale;
            SearchBox.FontSize = 8 * Scale;
            SpellList.FontSize = 7 * Scale;
            SpellName.FontSize = 16 * Scale;
            SpellLevel.FontSize = 9 * Scale;
            SpellSchool.FontSize = 9 * Scale;
            SpellCastTime.FontSize = 9 * Scale;
            SpellDuration.FontSize = 9 * Scale;
            SpellRange.FontSize = 9 * Scale;
            SpellInfo.FontSize = 9 * Scale;
            SpellDescription.FontSize = 9 * Scale;
            CharaTextBlock.FontSize = 7 * Scale;
            CreateChara.FontSize = 7 * Scale;
            RenameChara.FontSize = 7 * Scale;
            DeleteChara.FontSize = 7 * Scale;
            AddSpellsToTextBlock.FontSize = 7 * Scale;
            AddToThisClass.FontSize = 7 * Scale;
            AddSpell.FontSize = 7 * Scale;
            RemoveSpell.FontSize = 7 * Scale;
            Options.FontSize = 10 * Scale;

            SelectedClass.FontFamily = SettingsVariables.SelectedFont;
            SelectedLevel.FontFamily = SettingsVariables.SelectedFont;
            SelectedSchool.FontFamily = SettingsVariables.SelectedFont;
            SearchBox.FontFamily = SettingsVariables.SelectedFont;
            SpellList.FontFamily = SettingsVariables.SelectedFont;
            SpellName.FontFamily = SettingsVariables.SelectedFont;
            SpellLevel.FontFamily = SettingsVariables.SelectedFont;
            SpellSchool.FontFamily = SettingsVariables.SelectedFont;
            SpellCastTime.FontFamily = SettingsVariables.SelectedFont;
            SpellDuration.FontFamily = SettingsVariables.SelectedFont;
            SpellRange.FontFamily = SettingsVariables.SelectedFont;
            SpellInfo.FontFamily = SettingsVariables.SelectedFont;
            SpellDescription.FontFamily = SettingsVariables.SelectedFont;
            CharaTextBlock.FontFamily = SettingsVariables.SelectedFont;
            CreateChara.FontFamily = SettingsVariables.SelectedFont;
            RenameChara.FontFamily = SettingsVariables.SelectedFont;
            DeleteChara.FontFamily = SettingsVariables.SelectedFont;
            AddSpellsToTextBlock.FontFamily = SettingsVariables.SelectedFont;
            AddToThisClass.FontFamily = SettingsVariables.SelectedFont;
            AddSpell.FontFamily = SettingsVariables.SelectedFont;
            RemoveSpell.FontFamily = SettingsVariables.SelectedFont;
            Options.FontFamily = SettingsVariables.SelectedFont;

            //SelectedClass.Foreground = SettingsVariables.TextColor;
            //SelectedLevel.Foreground = SettingsVariables.TextColor;
            //SelectedSchool.Foreground = SettingsVariables.TextColor;
            //SearchBox.Foreground = SettingsVariables.TextColor;
            //SpellList.Foreground = SettingsVariables.TextColor;
            //SpellName.Foreground = SettingsVariables.TextColor;
            //SpellLevel.Foreground = SettingsVariables.TextColor;
            //SpellSchool.Foreground = SettingsVariables.TextColor;
            //SpellCastTime.Foreground = SettingsVariables.TextColor;
            //SpellDuration.Foreground = SettingsVariables.TextColor;
            //SpellRange.Foreground = SettingsVariables.TextColor;
            //SpellInfo.Foreground = SettingsVariables.TextColor;
            //SpellDescription.Foreground = SettingsVariables.TextColor;
            //CharaTextBlock.Foreground = SettingsVariables.TextColor;
            //CreateChara.Foreground = SettingsVariables.TextColor;
            //RenameChara.Foreground = SettingsVariables.TextColor;
            //DeleteChara.Foreground = SettingsVariables.TextColor;
            //AddSpellsToTextBlock.Foreground = SettingsVariables.TextColor;
            //AddToThisClass.Foreground = SettingsVariables.TextColor;
            //AddSpell.Foreground = SettingsVariables.TextColor;
            //RemoveSpell.Foreground = SettingsVariables.TextColor;
            //Options.Foreground = SettingsVariables.TextColor;

            //SelectedClass.Background = SettingsVariables.BackgroundGradientBrush;
            //SelectedLevel.Background = SettingsVariables.BackgroundGradientBrush;
            //SelectedSchool.Background = SettingsVariables.BackgroundColor;
            //SearchBox.Background = SettingsVariables.BackgroundColor;
            //SpellList.Background = SettingsVariables.BackgroundColor;
            ////SpellName.Background = SettingsVariables.BackgroundColor;
            ////SpellLevel.Background = SettingsVariables.BackgroundColor;
            ////SpellSchool.Background = SettingsVariables.BackgroundColor;
            ////SpellCastTime.Background = SettingsVariables.BackgroundColor;
            ////SpellDuration.Background = SettingsVariables.BackgroundColor;
            ////SpellRange.Background = SettingsVariables.BackgroundColor;
            ////SpellInfo.Background = SettingsVariables.BackgroundColor;
            //SpellDescription.Background = SettingsVariables.BackgroundColor;
            ////CharaTextBlock.Background = SettingsVariables.BackgroundColor;
            //CreateChara.Background = SettingsVariables.ButtonBrush;
            //RenameChara.Background = SettingsVariables.ButtonBrush;
            //DeleteChara.Background = SettingsVariables.ButtonBrush;
            ////AddSpellsToTextBlock.Background = SettingsVariables.BackgroundColor;
            //AddToThisClass.Background = SettingsVariables.BackgroundGradientBrush;
            //AddToThisClass.BorderBrush = SettingsVariables.BackgroundColor;
            //AddSpell.Background = SettingsVariables.ButtonBrush;
            //RemoveSpell.Background = SettingsVariables.ButtonBrush;
            //Options.Background = SettingsVariables.ButtonBrush;
            //mainWindow.Background = SettingsVariables.BackgroundColor;

            BottomSubGrid.ColumnDefinitions[1].Width = new GridLength(30 * Scale);
            BottomSubGrid.ColumnDefinitions[2].Width = new GridLength(30 * Scale);
            BottomSubGrid.ColumnDefinitions[3].Width = new GridLength(30 * Scale);
            BottomSubGrid.ColumnDefinitions[7].Width = new GridLength(30 * Scale);
            BottomSubGrid.ColumnDefinitions[8].Width = new GridLength(30 * Scale);

            BottomSubGrid.ColumnDefinitions[6].Width = new GridLength(100 * Scale);

            SearchBox.Visibility = SettingsVariables.ShowSearchBox ? Visibility.Visible : Visibility.Collapsed;
        }

        public static double Scale
        {
            get
            {
                return SettingsVariables.Scale;
            }
            set
            {
                SettingsVariables.Scale = value;
            }
        }

        private void SpellList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SpellList.SelectedItem == null)
            {
                return;
            }

            string spell = SpellList.SelectedItem.ToString();
            bool WasPrepared = spell[0] == '*';
            spell = spell.Trim('*');

            string Path = GetPreparedSpellPath(SelectedClass.SelectedItem.ToString());
            List<string> AllPreparedSpellsInClass = File.ReadAllLines(Path).ToList();
            if (IsPreparedSpellInCurrentClass(spell))
            {
                AllPreparedSpellsInClass.RemoveAll(z => z == spell);
            }
            else
            {
                AllPreparedSpellsInClass.Add(spell);
            }
            File.WriteAllLines(Path, AllPreparedSpellsInClass);
            LastClassLoadedIntoPrep = "";

            UpdateSpellListContents();
            if (!WasPrepared)
            {
                spell = $"*{spell}";
            }

            SpellList.SelectedIndex = SpellList.Items.IndexOf(spell);

        }

        private string LastClassLoadedIntoPrep = "";
        private List<string> AllPreparedSpells;

        private bool IsPreparedSpellInCurrentClass(string Spell)
        {

            string ClassName = SelectedClass.SelectedItem.ToString();
            if (LastClassLoadedIntoPrep == ClassName)
            {
                return AllPreparedSpells.Any(z => z == Spell);
            }


            string Base = GetPreparedSpellPath(ClassName);

            if (File.Exists(Base))
            {
                AllPreparedSpells = File.ReadAllLines(Base).ToList();
                LastClassLoadedIntoPrep = ClassName;
                return AllPreparedSpells.Any(z => z == Spell);
            }

            File.WriteAllText(Base, "");
            {
                return false;
            }
        }

        private static string GetPreparedSpellPath(string ClassName)
        {
            if (File.Exists($"classes/{ClassName}.dndbClass"))
            {
                return $"classes/{ClassName}.dndbClass.prep";
            }

            if (File.Exists($"classes/{ClassName}.dndbChara"))
            {
                return $"classes/{ClassName}.dndbChara.prep";
            }

            throw new FileNotFoundException("This class doesn't exist");
        }

        private void SearchBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateSpellListContents();
        }

        private void AddToThisClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    public class MasterSpell
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Level")]
        public string Level { get; set; }

        [JsonProperty("School")]
        public string School { get; set; }

        [JsonProperty("Casting_x0020_Time")]
        public string CastingX0020Time { get; set; }

        [JsonProperty("Duration")]
        public string Duration { get; set; }

        [JsonProperty("Range")]
        public string Range { get; set; }

        [JsonProperty("Area")]
        public string Area { get; set; }

        [JsonProperty("Ritual")]
        public string Ritual { get; set; }

        [JsonProperty("Concentration")]
        public string Concentration { get; set; }

        [JsonProperty("V")]
        public string V { get; set; }

        [JsonProperty("S")]
        public string S { get; set; }

        [JsonProperty("M")]
        public string M { get; set; }

        [JsonProperty("Material")]
        public string Material { get; set; }

        [JsonProperty("Details")]
        public string Details { get; set; }
    }

    public class Root
    {
        [JsonProperty("Master_Spells")]
        public List<MasterSpell> MasterSpells { get; set; }
    }


}