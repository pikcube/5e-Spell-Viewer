using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DnDB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var Existing = Directory.GetFiles("classes");
            if (Existing.Any(z => z == $"{textBox1.Text}.dndbClass") || Existing.Any(z => z == $"{textBox1.Text}.dndbChara"))
            {
                MessageBox.Show("Class Already Exists");
                return;
            }

            if (textBox1.Text == "")
            {
                MessageBox.Show("Enter a class name");
                return;
            }

            using (var writer = new StreamWriter($@"classes\{textBox1.Text}.dndbChara", false))
            {
                writer.Write("");
            }

            if (checkBox1.Checked)
            {
                if (File.Exists($@"classes\{comboBox1.Text}.dndbClass"))
                {
                    File.AppendAllLines($@"classes\{textBox1.Text}.dndbChara", File.ReadAllLines($@"classes\{comboBox1.Text}.dndbClass"));
                }

                if (File.Exists($@"classes\{comboBox1.Text}.dndbChara"))
                {
                    File.AppendAllLines($@"classes\{textBox1.Text}.dndbChara", File.ReadAllLines($@"classes\{comboBox1.Text}.dndbChara"));
                }
            }

            MainWindow.NewClass = textBox1.Text;
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<string> comboBox1DataSource = MainWindow.Classes.Select(z => z.ClassName).ToList();
            comboBox1DataSource.RemoveAt(0);
            comboBox1.DataSource = comboBox1DataSource;
        }
    }
}
