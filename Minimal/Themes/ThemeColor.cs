using Minimal.External;

namespace Minimal.Themes
{
    /// <summary>
    /// ThemeColor class saves 3 basic colors which are later used on control draw.
    /// </summary>
    public class ThemeColor
    {
        /// <summary>
        /// Normal color. Used most time.
        /// </summary>
        public Hex Normal;

        /// <summary>
        /// Hover color. Used when mouse cursor hovers control.
        /// </summary>
        public Hex Hover;

        /// <summary>
        /// Focus color. Control is focused.
        /// </summary>
        public Hex Focus;

        /// <summary>
        /// Disabled color. Control is not enabled.
        /// </summary>
        public Hex Disabled;
    }
}
