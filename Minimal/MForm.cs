using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Minimal.Components;
using Minimal.Themes;

namespace Minimal
{
    /// <summary>
    /// Minimal Form
    /// </summary>
    public partial class MForm : Form
    {
        /// <summary>
        /// Used theme local variable
        /// </summary>
        private Theme _theme;

        /// <summary>
        /// UsedTheme changed event
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when Theme is changed.")]
        public event PropertyChangedEventHandler ThemeChanged;

        /// <summary>
        /// Used theme property
        /// </summary>
        public Theme Theme
        {
            get { return _theme; }
            set
            {
                // Change used theme
                _theme = value;

                // Fire event
                ThemeChanged?.Invoke(this, new PropertyChangedEventArgs("ThemeChanged"));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MForm()
        {
            InitializeComponent();

            // Default theme
            Theme = M.Light;
        }
    }
}
