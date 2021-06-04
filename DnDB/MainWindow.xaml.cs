using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DnDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private DnDBDataSet db = new DnDBDataSet();
        static public DnDBDataSetTableAdapters.Master_SpellsTableAdapter TableAdapter;
        static public DnDBDataSet.Master_SpellsDataTable SpellTable;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SpellLevel.Text = $"Spell Level:{Environment.NewLine}Level 1";
            SpellSchool.Text = $"School:{Environment.NewLine}SomeDnBS";
            SpellCastTime.Text = $"Casting Time{Environment.NewLine}1 Action";
            SpellDuration.Text = $"Duration:{Environment.NewLine}1 Minute";
            SpellRange.Text = $"Range:{Environment.NewLine}Forever";
            SpellTable = new DnDBDataSet.Master_SpellsDataTable();
            TableAdapter = new DnDBDataSetTableAdapters.Master_SpellsTableAdapter();
            TableAdapter.Fill(SpellTable);
            SpellList.ItemsSource = SpellTable.Select(z => z.Name);
            SpellList.SelectedIndex = 0;
            SpellDescription.TextWrapping = TextWrapping.Wrap;

        }

        private void SpellList_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        public class SpellRow
        {
            public string Name;
            public string Level;
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
                    string otherInfo = "";
                    if (IsRitual)
                    {
                        otherInfo += "Is a Ritual, ";
                    }
                    if (IsConcentration)
                    {
                        otherInfo += "Requires Concentration, ";
                    }

                    if (Area != "")
                    {
                        otherInfo += $"Area: {Area}, ";
                    }

                    if (otherInfo != "")
                    {
                        otherInfo += Environment.NewLine;
                    }
                    otherInfo += $"Components: {VSM(IsVerbal, IsSomatic, IsMaterial)}";
                    if (IsMaterial)
                    {
                        otherInfo += $", {Material}";
                    }

                    return otherInfo;
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
                    Row.Level.ToString(),
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



            private SpellRow(string name, string level, string school, string castingTime, string duration, string range, string area,
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

            private SpellRow()
            {
            }
        }
    }
}