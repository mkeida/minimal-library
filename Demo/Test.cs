using Minimal;
using Minimal.Scaling;
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
    public partial class Test : MForm
    {
        public Test()
        {
            InitializeComponent();

            MessageBox.Show(DIP.Set(100).ToString());
        }

        private void mButton1_Click(object sender, EventArgs e)
        {
            if (Theme == M.Dark)
                M.SetTheme(this, M.Light);
            else
                M.SetTheme(this, M.Dark);
        }
    }
}
