using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Minimal.Components
{
    /// <summary>
    /// Popup form
    /// </summary>
    public partial class ContextForm : MForm
    {
        /// <summary>
        /// Color of bottom border
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// True if pop-form should hide on deactivate
        /// </summary>
        public bool HideOnDeactivate { get; set; } = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public ContextForm()
        {
            // Initialize form
            InitializeComponent();

            // Modify form
            // Manual start position
            StartPosition = FormStartPosition.Manual;

            // Border style
            FormBorderStyle = FormBorderStyle.None;

            // Hides form from task-bar
            ShowInTaskbar = false;            

            // Make window top-most
            TopMost = true;
        }

        /// <summary>
        /// On deactivate
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDeactivate(EventArgs e)
        {
            // Hides form
            if (HideOnDeactivate)
                Hide();
        }

        /// <summary>
        /// Shows form without stealing focus from main window
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            // Returns always true
            get { return true; }
        }

        /// <summary>
        /// Paint method
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base call
            base.OnPaint(e);

            // Draw bottom border
            using (Pen pen = new Pen(BorderColor))
                e.Graphics.DrawLine(pen, new Point(0, Height - 1), new Point(Width - 1, Height - 1));
        }

        /// <summary>
        /// Sent to a window in order to determine what part of the window 
        /// corresponds to a particular screen coordinate.
        /// </summary>
        private const int WM_NCHITTEST = 0x84;

        /// <summary>
        /// Client area
        /// </summary>
        private const int HTCLIENT = 0x1;

        /// <summary>
        /// Title bar
        /// </summary>
        private const int HTCAPTION = 0x2;

        /// <summary>
        /// True if aero is enabled
        /// </summary>
        private bool _aero;

        /// <summary>
        /// Shadow
        /// </summary>
        private const int CS_DROPSHADOW = 0x00020000;

        /// <summary>
        /// The WM_NCPAINT message is sent to a window when its frame must be painted.
        /// </summary>
        private const int WM_NCPAINT = 0x0085;

        /// <summary>
        /// Sent when a window belonging to a different application than the 
        /// active window is about to be activated.
        /// </summary>
        private const int WM_ACTIVATEAPP = 0x001C;

        /// <summary>
        /// Imports DwmExtendFrameIntoClientArea
        /// </summary>
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        /// <summary>
        /// Imports DwmSetWindowAttribute
        /// </summary>
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        /// <summary>
        /// Imports DwmIsCompositionEnabled
        /// </summary>
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        /// <summary>
        /// Imports CreateRoundRectRgn
        /// </summary>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse );

        /// <summary>
        /// Window margins
        /// </summary>
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        /// <summary>
        /// Create parameters override
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                // Update aero status
                _aero = CheckAeroEnabled();

                // Create parameters
                CreateParams cp = base.CreateParams;

                // Is aero activated?
                if (!_aero)
                    cp.ClassStyle |= CS_DROPSHADOW;

                // Returns parameters
                return cp;
            }
        }

        /// <summary>
        /// Checks if aero is enabled
        /// </summary>
        private bool CheckAeroEnabled()
        {
            // Checks OS version
            if (Environment.OSVersion.Version.Major >= 6)
            {
                // Default value
                int enabled = 0;

                // Check is aero is enabled
                DwmIsCompositionEnabled(ref enabled);

                // Returns true if aero is enabled
                return (enabled == 1) ? true : false;
            }

            // Returns false
            return false;
        }

        /// <summary>
        /// Window processing
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            // Check for message
            switch (m.Msg)
            {
                // When window must be painted
                case WM_NCPAINT:
                    // Is aero enabled?
                    if (_aero)
                    {
                        // Reference var
                        var v = 2;

                        // Sets window attribute
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);

                        // Create margins
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 0,
                            rightWidth = 0,
                            topHeight = 0
                        };

                        // Extends frame
                        DwmExtendFrameIntoClientArea(this.Handle, ref margins);                       
                    }

                    // End
                    break;

                // Default
                default:
                    break;
            }

            // Call windows processing with modified message
            base.WndProc(ref m);
        }
    }
}
