using System;
using Minimal.Themes;

namespace Minimal.Components.Core
{
    /// <summary>
    /// Theme changed event arguments
    /// </summary>
    public class ThemeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Theme
        /// </summary>
        public Theme Theme { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ThemeChangedEventArgs(Theme theme)
        {
            Theme = theme;
        }
    }
}
