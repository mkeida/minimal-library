using Minimal;
using Minimal.Components.Items;
using Minimal.External;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class Doc : Form
    {
        Main m;

        public Doc(Main main)
        {
            InitializeComponent();

            BackColor = new Hex("#F8F8F8");

            m = main;
        }

        private void mButton1_Click(object sender, EventArgs e)
        {
            if (m.Theme == M.Light)
                M.SetTheme(m, M.Dark);
            else
                M.SetTheme(m, M.Light);
        }
    }
}
