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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = MainWindow.Characters.Select(z => z.ClassName).ToList();
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] Existing = Directory.GetFiles("classes");
            if (Existing.Any(z => z == $@"classes\{textBox1.Text}.dndbClass") || Existing.Any(z => z == $@"classes\{textBox1.Text}.dndbChara"))
            {
                MessageBox.Show("Character/Class Already Exists");
                return;
            }

            if (textBox1.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                MessageBox.Show($"Names can't contain {string.Join("", Path.GetInvalidFileNameChars())}");
                return;
            }

            if (textBox1.Text == "")
            {
                MessageBox.Show("Enter a new character name");
                return;
            }

            string[] a = File.ReadAllLines($@"classes\{comboBox1.Text}.dndbChara");
            File.WriteAllLines($@"classes\{textBox1.Text}.dndbChara", a);
            File.Delete($@"classes\{comboBox1.Text}.dndbChara");

            MainWindow.NewClass = textBox1.Text;
            Close();
        }
    }
}
