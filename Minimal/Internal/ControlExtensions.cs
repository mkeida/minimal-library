using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace Minimal.Internal
{
    /// <summary>
    /// Internal class. Helps with bugged (bad written) default WinForms controls
    /// </summary>
    internal static class ControlExtensions
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 11;

        /// <summary>
        /// Stop redrawing
        /// </summary>
        public static void BeginUpdate(Control control)
        {
            try { SendMessage(control.Handle, WM_SETREDRAW, false, 0); } catch { }
        }

        /// <summary>
        /// Start redraw
        /// </summary>
        public static void EndUpdate(Control control)
        {
            try
            {
                SendMessage(control.Handle, WM_SETREDRAW, true, 0);
                control.Refresh();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Get ALL child controls
        /// </summary>
        public static IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAll(ctrl, type)).Concat(controls).Where(c => c.GetType() == type);
        }

        /// <summary>
        /// Cascade through all child controls of control and set property
        /// </summary>
        public static void SetProperty(Control control, string prop, object value)
        {
            // Set property
            if (control.GetType().GetProperty(prop) != null)
            {
                control.GetType().GetProperty(prop).SetValue(control, value);
            }

            // Cascade
            foreach (Control child in control.Controls)
            {
                SetProperty(child, prop, value);
            }
        }
    }
}
