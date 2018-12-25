using Minimal.External;
using Minimal.Themes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minimal.Components.Core
{
    /// <summary>
    /// MComponent
    /// </summary>
    public class MComponent
    {
        /// <summary>
        /// Component's accent property
        /// </summary>
        public Color Accent { get; set; } = Hex.blue;

        /// <summary>
        /// Component's custom theme local variable
        /// </summary>
        private Theme _customTheme;

        /// <summary>
        /// Component's custom theme property
        /// </summary>
        public Theme CustomTheme
        {
            get { return _customTheme; }
            set
            {
                if (value != _customTheme)
                {
                    // Update local variable
                    _customTheme = value;

                    // Triggers theme changed to ensure component source-theme update
                    InvokeThemeChanged(SourceTheme);
                }
            }
        }

        /// <summary>
        /// Source theme used for actual component rendering
        /// </summary>
        public Theme SourceTheme { get; set; }

        /// <summary>
        /// True if control is outdated and should be redrawn
        /// </summary>
        public bool Outdated { get; set; }

        /// <summary>
        /// Component's parent form
        /// </summary>
        public Form ParentForm { get; set; }

        /// <summary>
        /// Component's update event handler
        /// </summary>
        public event EventHandler ComponentUpdate;

        /// <summary>
        /// Component's input type event handler
        /// </summary>
        public event EventHandler<InputTypeChangedEventArgs> InputTypeChanged;

        /// <summary>
        /// Component's theme update event handler
        /// </summary>
        public event EventHandler<ThemeChangedEventArgs> ThemeChanged;

        /// <summary>
        /// Triggers component update
        /// </summary>
        internal void InvokeComponentUpdate()
        {
            // Invoke component's update event
            ComponentUpdate?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggers theme change
        /// </summary>
        internal void InvokeThemeChanged(Theme theme)
        {
            // Invoke component's theme-changed event
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme));
        }

        /// <summary>
        /// Triggers theme change
        /// </summary>
        internal void InvokeInputTypeChanged(bool touchInput)
        {
            // Invoke component's theme-changed event
            InputTypeChanged?.Invoke(this, new InputTypeChangedEventArgs(touchInput));
        }
    }
}
