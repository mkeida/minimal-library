using Minimal;
using Minimal.Components;
using Minimal.Components.Items;
using Minimal.External;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class Main : MForm
    {
        /// <summary>
        /// Performance counter
        /// </summary>
        PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        /// <summary>
        /// Chart steps
        /// </summary>
        int x = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public Main()
        {
            InitializeComponent();

            // Set background color
            BackColor = Color.White;

            // Set key preview on
            KeyPreview = true;

            // Accent
            M.SetAccent(this, M.GetWindowsActiveColor());

            Doc d = new Doc(this);
            d.Show();
        }

        /// <summary>
        /// Update - every 1 second
        /// </summary>
        private void Update(object sender, EventArgs e)
        {
            // CPU usage
            int num = (int) cpuCounter.NextValue();

            // Add new CPU usage value
            cpuChart.Values.Add(new ChartValue(x, num));

            // Increment steps
            x += 1;

            // Update usage
            lbUsage.Text = $"{num}%";

            // Update processes
            lbProcesses.Text = Process.GetCurrentProcess().HandleCount.ToString();

            // Update threads
            lbThreads.Text = Process.GetCurrentProcess().Threads.Count.ToString();
        }


        /// <summary>
        /// On window resize
        /// </summary>
        private void OnResize(object sender, EventArgs e)
        {
            cpuChart.Width = Width - 58;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // if (e.KeyCode == Keys.Escape)
                // drawer.Opened = !drawer.Opened;
        }
    }
}
