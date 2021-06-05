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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var Existing = Directory.GetFiles("classes");
            if (Existing.Any(z => z == $"{textBox1.Text}.dndbClass"))
            {
                MessageBox.Show("Class Already Exists");
                return;
            }

            if (textBox1.Text == "")
            {
                MessageBox.Show("Enter a class name");
                return;
            }

            using (var writer = new StreamWriter($@"classes\{textBox1.Text}.dndbClass", false))
            {
                writer.Write("");
            }

            MainWindow.NewClass = textBox1.Text;

            Close();
        }
    }
}
