using System;
using Minimal.Themes;

namespace Minimal.Components.Core
{
    /// <summary>
    /// Theme changed event arguments
    /// </summary>
    public class InputTypeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// True if touch is enabled
        /// </summary>
        public bool TouchEnabled { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public InputTypeChangedEventArgs(bool touchEnabled)
        {
            TouchEnabled = touchEnabled;
        }
    }
}
