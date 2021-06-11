using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace DnDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static List<DnDBClass> Classes = new List<DnDBClass>();
        public static List<DnDBClass> Characters = new List<DnDBClass>();
        private static DnDBDataSetTableAdapters.Master_SpellsTableAdapter TableAdapter;
        private static DnDBDataSet.Master_SpellsDataTable SpellTable;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //SpellLevel.Text = $"Spell Level:{Environment.NewLine}Level 1";
            //SpellSchool.Text = $"School:{Environment.NewLine}SomeDnBS";
            //SpellCastTime.Text = $"Casting Time{Environment.NewLine}1 Action";
            //SpellDuration.Text = $"Duration:{Environment.NewLine}1 Minute";
            //SpellRange.Text = $"Range:{Environment.NewLine}Forever";
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

            SpellTable = new DnDBDataSet.Master_SpellsDataTable();
            TableAdapter = new DnDBDataSetTableAdapters.Master_SpellsTableAdapter();
            TableAdapter.Fill(SpellTable);
            SpellDescription.TextWrapping = TextWrapping.Wrap;



            UpdateClasses();
        }

        private void SelectedClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SpellList.ItemsSource = SpellTable.Select(z => z.Name).Where(z =>
            {
                SpellRow Spell = SpellRow.GetSpell(z);

                return Classes[SelectedClass.SelectedIndex].Spells.Any(x => x.Name == Spell.Name) &&
                       (Spell.Level + 1 == SelectedLevel.SelectedIndex || SelectedLevel.SelectedIndex == 0) &&
                       (Spell.School == SelectedSchool.SelectedItem.ToString() || SelectedSchool.SelectedIndex == 0);
            });
            SpellList.SelectedIndex = 0;
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
                string[] Spells = File.ReadAllLines(Path);
                for (int n = 0; n < Spells.Length; ++n)
                {
                    Spells[n] = Spells[n].Trim('\"');
                }
                return new DnDBClass(string.Concat(System.IO.Path.GetFileName(Path).Reverse().Skip(10).Reverse()), Spells);
            }

            public DnDBClass(string name, string[] spells)
            {
                ClassName = name;
                Spells = new List<SpellRow>();
                foreach (var S in spells)
                {
                    Spells.Add(SpellRow.GetSpell(S));
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

                return Output;
            }

            public static SpellRow GetSpell(string SpellName)
            {
                DnDBDataSet.Master_SpellsRow Row = SpellTable.Rows.Find(SpellName) as DnDBDataSet.Master_SpellsRow;
                if (Row == null)
                {
                    throw new Exception("Can't Find The Spell");
                }

                return new SpellRow(
                    Row.Name,
                    Row.Level,
                    Row.School,
                    Row.Casting_Time,
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



            private SpellRow(string name, int level, string school, string castingTime, string duration, string range, string area,
                bool isRitual, bool isConcentration, bool isVerbal, bool isSomatic, bool isMaterial, string material, string details)
            {
                Name = name;
                Level = level;
                School = school;
                CastingTime = castingTime;
                Duration = duration;
                Range = range;
                Area = area == "n/a" ? "" : area;
                IsRitual = isRitual;
                IsConcentration = isConcentration;
                IsVerbal = isVerbal;
                IsSomatic = isSomatic;
                IsMaterial = isMaterial;
                Material = material == "none" ? "" : material;
                Details = details;
            }
        }

        private void AddSpell_Click(object sender, RoutedEventArgs e)
        {
            string SelectedSpell = (string)SpellList.SelectedItem;
            string AddToThisClassSelectedName = (string)AddToThisClass.SelectedItem;
            using (StreamWriter writer = new StreamWriter($@"classes\{AddToThisClassSelectedName}.dndbChara", true))
            {
                writer.WriteLine($"\"{SelectedSpell}\"");
            }
            UpdateClasses();
            MessageBox.Show($"{SelectedSpell} added to {AddToThisClassSelectedName}", "Add complete");
        }

        private void RemoveSpell_Click(object sender, RoutedEventArgs e)
        {
            string SelectedSpell = (string)SpellList.SelectedItem;
            string AddToThisClassSelectedName = (string)AddToThisClass.SelectedItem;
            MessageBoxResult Result = MessageBox.Show($"Are you sure you want to remove {SelectedSpell} from {AddToThisClassSelectedName}?", "Caution", MessageBoxButton.YesNo);
            if (Result != MessageBoxResult.Yes)
            {
                return;
            }
            DnDBClass Class = Classes.First(z => z.ClassName == AddToThisClassSelectedName);
            Class.Spells.RemoveAll(z => z.Name == SelectedSpell);
            using (StreamWriter writer = new StreamWriter($@"classes\{AddToThisClassSelectedName}.dndbChara", false))
            {
                foreach (string spell in Class.Spells.Select(z => z.Name))
                {
                    writer.WriteLine($"\"{spell}\"");
                }
            }
            UpdateClasses();
            MessageBox.Show($"{SelectedSpell} removed from {AddToThisClassSelectedName}", "Remove complete");
        }

        public static string NewClass = "";

        private void CreateChara_Click(object sender, RoutedEventArgs e)
        {
            Form1 F = new Form1 { StartPosition = FormStartPosition.CenterParent };
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
            Form2 F = new Form2 { StartPosition = FormStartPosition.CenterParent };
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
            Form3 F = new Form3 { StartPosition = FormStartPosition.CenterParent };
            F.ShowDialog();
            UpdateClasses();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectedLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}