using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Minimal.Components.Core;

namespace Minimal.Components
{
    /// <summary>
    /// Stack panel control.
    /// </summary>
    public partial class MStackPanel : TabControl, IMComponent
    {
        /// <summary>
        /// Component
        /// </summary>
        public MComponent Component { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MStackPanel()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialize component
            InitializeComponent();

            // Enables double buffer
            DoubleBuffered = true;
        }

        /// <summary>
        /// Occurs when a handle is created for the control. Handles
        /// event hooking.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            // Base call
            base.OnHandleCreated(e);

            // Hook up theme changed method
            Component.ThemeChanged += OnThemeChanged;
        }


        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            // Iterate over all tabs
            foreach (TabPage page in TabPages)
            {
                // Change color
                page.BackColor = e.Theme.FORM_BACKGROUND.Normal;
            }
        }

        /// <summary>
        /// Hide tabs
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x1328 && !DesignMode)
                m.Result = (IntPtr)1;
            else
                base.WndProc(ref m);
        }
    }
}
