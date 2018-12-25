using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Minimal.Components.Core;

namespace Minimal.Components
{
    /// <summary>
    /// Double buffered panel
    /// </summary>
    public partial class MBufferedPanel : Panel, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        public MComponent Component { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MBufferedPanel()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Turn on double buffer
            DoubleBuffered = true;

            // Redraw control
            Invalidate();
        }

        // Hide default panel's scroll-bars
        /// <summary>
        /// User32 .dll
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="wBar">Scrollbar</param>
        /// <param name="bShow">Show</param>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        /// <summary>
        /// Scroll bar directions
        /// </summary>
        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        /// <summary>
        /// Process call back
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            // If we are not in design mode
            if (!DesignMode)
            {
                // Hide scrollbar
                ShowScrollBar(this.Handle, (int)ScrollBarDirection.SB_BOTH, false);
            }

            // Call window process callback
            base.WndProc(ref m);
        }
    }
}
