using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
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

                if (Contents.Length == 6)
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
                if (path is null)
                {
                    throw new NoNullAllowedException();
                }
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
                false.ToString(),
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

        public bool DarkModeEnabled
        {
            get => bool.Parse(Contents[5]);
            set
            {
                Contents[5] = value.ToString();
                SaveOptionsConfig();
            }
        }

        public Style DarkStyle;
        public Style LightStyle;

        public Style ComboStyle => DarkModeEnabled ? DarkStyle : LightStyle;

        public Brush TextColor => DarkModeEnabled ? Brushes.White : Brushes.Black;

        public Brush BackgroundColor => DarkModeEnabled ? new SolidColorBrush(Color.FromRgb(24, 27, 23)) : Brushes.White;

        public LinearGradientBrush BackgroundGradientBrush => DarkModeEnabled
            ? new LinearGradientBrush(GradientStopCollection.Parse("#0036393F,0 #0036393F,1"))
            : new LinearGradientBrush(GradientStopCollection.Parse("#FFF0F0F0,0 #FFE5E5E5,1"));
        
        public Brush ButtonBrush
        {
            get => DarkModeEnabled ? new SolidColorBrush(Color.FromArgb(255, 34, 34, 34)) : new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
        }

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
        private static List<MasterSpell> SpellTable;

        public static string SelectedClassName;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Classes = new List<DnDBClass>();
                Characters = new List<DnDBClass>();
                string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                try
                {
                    Directory.SetCurrentDirectory(
                        $"{AppDataPath}/DnDB_Beta");
                }
                catch (IOException)
                {
                    Directory.CreateDirectory(
                        $"{AppDataPath}/DnDB_Beta");
                    Directory.SetCurrentDirectory(
                        $"{AppDataPath}/DnDB_Beta");
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

                SettingsVariables.DarkStyle = SelectedClass.Style;
                SettingsVariables.LightStyle = SelectedSchool.Style;

                RefreshSpells();

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

        private static void RefreshSpells()
        {
            Root I = JsonConvert.DeserializeObject<Root>(File.ReadAllText("spells.json"));
            SpellTable = I.MasterSpells;

            if (File.Exists("spells.custom.json"))
            {
                I = JsonConvert.DeserializeObject<Root>(File.ReadAllText("spells.custom.json"));
                I.MasterSpells.RemoveAll(z => z.Name == null);
                SpellTable.AddRange(I.MasterSpells);
                SpellTable = SpellTable.Distinct().ToList();
            }
            else
            {
                CreateCustomJson();
            }

            File.WriteAllLines(@"classes\Any Spell.dndbClass", SpellTable.Select(z => z.Name));
        }

        public static void CreateCustomJson()
        {
            Root R = new Root
            {
                MasterSpells = new List<MasterSpell> { new MasterSpell(), },
            };
            File.WriteAllText("spells.custom.json", JsonConvert.SerializeObject(R, Formatting.Indented));
            MasterSpell Example = new MasterSpell
            {
                Name = "Any text",
                Level = "Single digit 0-9",
                School = "Name of school",
                CastingTime = "Any text",
                Duration = "Any text",
                Range = "Any text",
                Area = "Any text",
                Ritual = "1 for ritual casting allowed, 0 otherwise",
                Concentration = "1 for requires concentration, 0 otherwise",
                V = "1 for requires verbal components, 0 otherwise",
                S = "1 for requires somatic components, 0 otherwise",
                M = "1 for requires material components, 0 otherwise",
                Material = "The names of any materials required",
                Details = "Any text, use \n for new lines. No data can be null. " +
                          "'Spells with null names won't be loaded. Any other null properties may crash the app",
            };
            MasterSpell Example2 = new MasterSpell
            {
                Name = "Second Spell",
                Level = "3",
                School = "Evocation",
                CastingTime = "1 Bonus Action",
                Duration = "Instantanious",
                Range = "Self",
                Area = "50 Feet",
                Ritual = "0",
                Concentration = "0",
                V = "1",
                S = "0",
                M = "0",
                Material = null,
                Details = "Material can be set to null as long as M is not set to 1 because the data in material is ignored " +
                          "if Material is decoded as false. However, it is recommended to put in a dummy string to prevent " +
                          "crashing in the case of M being set to 1 by accident.",
            };
            R.MasterSpells.Clear();
            R.MasterSpells.Add(Example);
            R.MasterSpells.Add(Example2);
            File.WriteAllText("README.example.spells.custom.json", JsonConvert.SerializeObject(R, Formatting.Indented));
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

        private int ManualCounter;
        private int _selectedClassPriorIndex = 0;

        private int SelectedClassPriorIndex
        {
            get
            {
                return _selectedClassPriorIndex;
            }
            set
            {
                if (value + 1 != SelectedClass.Items.Count)
                {
                    _selectedClassPriorIndex = value;
                }
            }
        }

        private void SelectedClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedClassPriorIndex = SelectedClass.SelectedIndex;

            UpdateSpellListContents();

            SelectedClassName = SelectedClass.SelectedItem.ToString();
            if (ManualControl)
            {
                return;
            }

            if (AddToThisClass.Items.Cast<string>().Any(z => z == (string)SelectedClass.SelectedItem))
            {
                AddToThisClass.SelectedItem = SelectedClass.SelectedItem;
                ManualCounter = 1;
            }
            else if (ManualCounter == 1)
            {
                ManualControl = true;
                ManualCounter = 2;
            }
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
                SpellsToFilter = SpellTable.Select(z => SpellRow.GetSpell(z.Name)).Where(z => //Create a list of spells where
                    Classes[SelectedClass.SelectedIndex].Spells.Any(x => x.Name == z.Name) && //The spell is in the selected class
                    (z.Level + 1 == SelectedLevel.SelectedIndex || SelectedLevel.SelectedIndex == 0) && //The spell is the correct level
                    (z.School == SelectedSchool.SelectedItem.ToString() || SelectedSchool.SelectedIndex == 0)); //The spell is in the correct school
            }
            catch (Exception)
            {
                RestoreDefaultClassFiles();
                return;
            }

            if (SearchBox.Text != string.Empty) //If the search box has text in it
            {
                SpellsToFilter = SpellsToFilter.Where(z => z.Name.ToLower().Contains(SearchBox.Text.ToLower())); //Spell matches search box
            }

            //Apparently you can't update a member of an IEnumberable from a foreach loop, it just updates a local copy and throws it away.
            List<SpellRow> SpellsFiltered = SpellsToFilter.ToList();

            foreach (SpellRow s in SpellsFiltered.Where(s => IsPreparedSpellInCurrentClass(s.Name))) //Add a * to any prepared spells
            {
                s.Name = $"*{s.Name}";
            }

            if (SettingsVariables.SortByLevel) //Sort spells first by prepared, then by level, then by name
            {
                SpellList.ItemsSource = SpellsFiltered
                    .OrderBy(z => z.Name[0] != '*') //False shows up first before true, so this check is inverted. Given 1 = true, makes sense
                    .ThenBy(z => z.Level)
                    .ThenBy(z => z.Name).Select(z => z.Name);
            }
            else //Don't sort by level
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
            OldIndex1 = SelectedClass.SelectedIndex + 1 >= SelectedClass.Items.Count
                ? Math.Max(SelectedClass.Items.Count - 1, 0)
                : OldIndex1;
            int OldIndex2 = AddToThisClass.SelectedIndex == -1 ? 0 : AddToThisClass.SelectedIndex;
            OldIndex2 = AddToThisClass.SelectedIndex >= AddToThisClass.Items.Count
                ? AddToThisClass.Items.Count - 1
                : OldIndex2;

            SelectedClass.ItemsSource = Classes.Select(z => z.ClassName);

            SelectedClass.SelectedIndex = 0;

            SelectedClass.SelectedIndex = Math.Min(OldIndex1, SelectedClass.Items.Count - 1);




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

            MenuItem AddRightClickItem = RightClickSpellList.Items[1] as MenuItem;
            MenuItem RemoveRightClickItem = RightClickSpellList.Items[2] as MenuItem;
            AddRightClickItem.Items.Clear();
            RemoveRightClickItem.Items.Clear();

            foreach (DnDBClass C in Characters)
            {
                MenuItem T = new MenuItem
                {
                    Header = C.ClassName
                };
                T.Click += AddClick;
                MenuItem I = new MenuItem
                {
                    Header = C.ClassName
                };
                I.Click += RemoveClick;
                AddRightClickItem.Items.Add(T);
                RemoveRightClickItem.Items.Add(I);
            }

            return;
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            MenuItem a = sender as MenuItem;
            AddToThisClass.SelectedItem = a.Header.ToString();
            RemoveSpellFromCharacter(a.Header.ToString());
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            MenuItem a = sender as MenuItem;
            AddToThisClass.SelectedItem = a.Header.ToString();
            AddSpellToCharacter(a.Header.ToString());
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
            SpellRow Spell = SpellRow.GetSpell((string)SpellList.SelectedItem);
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
                for (int index = 0; index < Spells.Count; index++)
                {
                    if (Spells[index] == "Tasha's Otherworldy Guise")
                    {
                        Spells[index] = "Tasha's Otherworldly Guise";
                    }
                }

                File.WriteAllLines(Path, Spells);

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
                            throw new Exception("Class File Corrupted\nLookup into Json File Failed");
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
            public string Details;
            public string Material;

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
                    Row.CastingTime,
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
            string AddToThisClassSelectedName = (string)AddToThisClass.SelectedItem;

            AddSpellToCharacter(AddToThisClassSelectedName);
        }

        private void AddSpellToCharacter(string AddToThisClassSelectedName)
        {
            if (SpellList.Items.Count == 0)
            {
                return;
            }

            if (SpellList.SelectedItems.Count == 0)
            {
                string SelectedSpell = "";
                if (!AttemptToInferSelectedSpell(ref SelectedSpell))
                {
                    return;
                }

                SpellList.SelectedItem = SelectedSpell;
            }


            List<string> SelectedItems = new List<string>();

            using (StreamWriter writer = new StreamWriter($@"classes\{AddToThisClassSelectedName}.dndbChara", true))
            {
                foreach (string SelectedSpell in SpellList.SelectedItems)
                {
                    writer.WriteLine(SelectedSpell.TrimStart('*'));
                    SelectedItems.Add(SelectedSpell);
                }
            }

            UpdateClasses();
            MessageBox.Show($"{string.Join(", ", SelectedItems)} added to {AddToThisClassSelectedName}", "Add complete");
        }

        private bool AttemptToInferSelectedSpell(ref string SelectedSpell)
        {
            if (!(SpellList.ItemsSource is IEnumerable<string> a))
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
            string AddToThisClassSelectedName = (string)AddToThisClass.SelectedItem;

            RemoveSpellFromCharacter(AddToThisClassSelectedName);
        }

        private void RemoveSpellFromCharacter(string AddToThisClassSelectedName)
        {
            if (SpellList.SelectedItems.Count == 0)
            {
                string SelectedSpell = "";
                if (!AttemptToInferSelectedSpell(ref SelectedSpell))
                {
                    return;
                }

                SpellList.SelectedItem = SelectedSpell;
            }

            List<string> SelectedItems = SpellList.SelectedItems.Cast<string>().ToList();

            MessageBoxResult Result =
                MessageBox.Show(
                    $"Are you sure you want to remove {string.Join(", ", SelectedItems)} from {AddToThisClassSelectedName}?",
                    "Caution", MessageBoxButton.YesNo);
            if (Result != MessageBoxResult.Yes)
            {
                return;
            }

            DnDBClass Class = Classes.First(z => z.ClassName == AddToThisClassSelectedName);
            foreach (string SelectedSpell in SelectedItems)
            {
                Class.Spells.RemoveAll(z => z.Name == SelectedSpell);
            }

            File.WriteAllLines($@"classes\{AddToThisClassSelectedName}.dndbChara", Class.Spells.Select(z => z.Name));
            UpdateClasses();
            MessageBox.Show($"{string.Join(", ", SelectedItems)} removed from {AddToThisClassSelectedName}",
                "Remove complete");
        }

        public static string NewClass = "";

        private void CreateChara_Click(object sender, RoutedEventArgs e)
        {
            //Form1 F = new Form1 { StartPosition = FormStartPosition.CenterParent };
            CreateChara F = new CreateChara { WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, };
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
            RenameChara F = new RenameChara { WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, };
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
            DeleteChara F = new DeleteChara { WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, };
            F.ShowDialog();
            UpdateClasses();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            Options O = new Options { WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, };
            O.ShowDialog();
            UpdateFontSize();
            RefreshSpells();
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
            SpellList.ContextMenu.FontSize = 7 * Scale;

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
            SpellList.ContextMenu.FontFamily = SettingsVariables.SelectedFont;

            SelectedClass.Foreground = SettingsVariables.TextColor;
            SelectedLevel.Foreground = SettingsVariables.TextColor;
            SelectedSchool.Foreground = SettingsVariables.TextColor;
            SearchBox.Foreground = SettingsVariables.TextColor;
            SpellList.Foreground = SettingsVariables.TextColor;
            SpellName.Foreground = SettingsVariables.TextColor;
            SpellLevel.Foreground = SettingsVariables.TextColor;
            SpellSchool.Foreground = SettingsVariables.TextColor;
            SpellCastTime.Foreground = SettingsVariables.TextColor;
            SpellDuration.Foreground = SettingsVariables.TextColor;
            SpellRange.Foreground = SettingsVariables.TextColor;
            SpellInfo.Foreground = SettingsVariables.TextColor;
            SpellDescription.Foreground = SettingsVariables.TextColor;
            CharaTextBlock.Foreground = SettingsVariables.TextColor;
            CreateChara.Foreground = SettingsVariables.TextColor;
            RenameChara.Foreground = SettingsVariables.TextColor;
            DeleteChara.Foreground = SettingsVariables.TextColor;
            AddSpellsToTextBlock.Foreground = SettingsVariables.TextColor;
            AddToThisClass.Foreground = SettingsVariables.TextColor;
            AddSpell.Foreground = SettingsVariables.TextColor;
            RemoveSpell.Foreground = SettingsVariables.TextColor;
            Options.Foreground = SettingsVariables.TextColor;
            Edit.Foreground = SettingsVariables.TextColor;

            SelectedClass.Style = SettingsVariables.ComboStyle;
            SelectedLevel.Style = SettingsVariables.ComboStyle;
            SelectedSchool.Style = SettingsVariables.ComboStyle;
            SpellList.Background = SettingsVariables.BackgroundColor;
            //SpellName.Background = SettingsVariables.BackgroundColor;
            //SpellLevel.Background = SettingsVariables.BackgroundColor;
            //SpellSchool.Background = SettingsVariables.BackgroundColor;
            //SpellCastTime.Background = SettingsVariables.BackgroundColor;
            //SpellDuration.Background = SettingsVariables.BackgroundColor;
            //SpellRange.Background = SettingsVariables.BackgroundColor;
            //SpellInfo.Background = SettingsVariables.BackgroundColor;
            SearchBox.Background = SettingsVariables.BackgroundColor;
            SpellDescription.Background = SettingsVariables.BackgroundColor;
            //CharaTextBlock.Background = SettingsVariables.BackgroundColor;
            CreateChara.Background = SettingsVariables.ButtonBrush;
            RenameChara.Background = SettingsVariables.ButtonBrush;
            DeleteChara.Background = SettingsVariables.ButtonBrush;
            //AddSpellsToTextBlock.Background = SettingsVariables.BackgroundColor;
            AddToThisClass.Style = SettingsVariables.ComboStyle;
            AddToThisClass.BorderBrush = SettingsVariables.BackgroundColor;
            AddSpell.Background = SettingsVariables.ButtonBrush;
            RemoveSpell.Background = SettingsVariables.ButtonBrush;
            Options.Background = SettingsVariables.ButtonBrush;
            mainWindow.Background = SettingsVariables.BackgroundColor;
            Edit.Background = SettingsVariables.ButtonBrush;

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

        private void MenuItemPrepare_OnClick(object sender, RoutedEventArgs e)
        {
            SpellList_MouseDoubleClick(sender, null);
        }

        private void SpellList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SpellList.SelectedItem == null)
            {
                return;
            }

            string Path = GetPreparedSpellPath(SelectedClass.SelectedItem.ToString());
            List<string> AllPreparedSpellsInClass = File.ReadAllLines(Path).ToList();
            string FirstSpell = SpellList.SelectedItem.ToString();
            bool WasPrepared = FirstSpell[0] == '*';


            foreach (string S in SpellList.SelectedItems.Cast<string>())
            {
                string spell = S;
                spell = spell.Trim('*');
                if (IsPreparedSpellInCurrentClass(spell))
                {
                    AllPreparedSpellsInClass.RemoveAll(z => z == spell);
                }
                else
                {
                    AllPreparedSpellsInClass.Add(spell);
                }
            }
            File.WriteAllLines(Path, AllPreparedSpellsInClass);
            LastClassLoadedIntoPrep = "";

            UpdateSpellListContents();
            if (!WasPrepared)
            {
                FirstSpell = $"*{FirstSpell}";
            }

            SpellList.SelectedIndex = SpellList.Items.IndexOf(FirstSpell);

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

        private bool ManualControl;

        private void AddToThisClass_GotFocus(object sender, RoutedEventArgs e)
        {
            ManualControl = true;

        }

        private void GridSplitter1_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridSplitter1.Height = mainWindow.ActualHeight;
        }

        public static int EditResult = -1;

        private void Edit_OnClick(object sender, RoutedEventArgs e)
        {
            EditCharacter m = new EditCharacter { WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, };
            m.ShowDialog();
            switch (EditResult)
            {
                case 0:
                    CreateChara_Click(sender, e);
                    break;
                case 1:
                    RenameChara_Click(sender, e);
                    break;
                case 2:
                    DeleteChara_Click(sender, e);
                    break;
            }

            EditResult = -1;
        }
    }

    public class MasterSpell
    {
        [JsonProperty("Name")]
        public string Name
        {
            get; 
            set;
        }

        [JsonProperty("Level")]
        public string Level
        {
            get; 
            set;
        }

        [JsonProperty("School")]
        public string School
        {
            get; 
            set;
        }

        [JsonProperty("CastingTime")]
        public string CastingTime
        {
            get; 
            set;
        }

        [JsonProperty("Duration")]
        public string Duration
        {
            get; 
            set;
        }

        [JsonProperty("Range")]
        public string Range
        {
            get; 
            set;
        }

        [JsonProperty("Area")]
        public string Area
        {
            get; 
            set;
        }

        [JsonProperty("Ritual")]
        public string Ritual
        {
            get; 
            set;
        }

        [JsonProperty("Concentration")]
        public string Concentration
        {
            get; 
            set;
        }

        [JsonProperty("V")]
        public string V 
        {
            get; 
            set;
        }

        [JsonProperty("S")]
        public string S
        {
            get; 
            set;
        }

        [JsonProperty("M")]
        public string M
        {
            get; 
            set;
        }

        [JsonProperty("Material")]
        public string Material
        {
            get; 
            set;
        }

        [JsonProperty("Details")]
        public string Details
        {
            get; 
            set;
        }
    }

    public class Root
    {
        [JsonProperty("Master_Spells")]
        public List<MasterSpell> MasterSpells
        {
            get; 
            set;
        }
    }


}