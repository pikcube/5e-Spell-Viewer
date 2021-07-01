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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] Existing = Directory.GetFiles("classes");

            File.Delete($@"classes\{comboBox1.Text}.dndbChara");

            Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = MainWindow.Characters.Select(z => z.ClassName).ToList();
        }
    }
}
