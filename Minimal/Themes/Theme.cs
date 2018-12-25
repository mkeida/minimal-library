namespace Minimal.Themes
{
    /// <summary>
    /// Represents library theme class
    /// </summary>
    public class Theme
    {
        /// <summary>
        /// Control background color
        /// </summary>
        public ThemeColor COMPONENT_BACKGROUND { get; set; }

        /// <summary>
        /// Control foreground color (text color)
        /// </summary>
        public ThemeColor COMPONENT_FOREGROUND { get; set; }

        /// <summary>
        /// Fill color of control
        /// </summary>
        public ThemeColor COMPONENT_FILL { get; set; }

        /// <summary>
        /// Control border color
        /// </summary>
        public ThemeColor COMPONENT_BORDER { get; set; }

        /// <summary>
        /// Control highlight color
        /// </summary>
        public ThemeColor COMPONENT_HIGHLIGHT { get; set; }

        /// <summary>
        /// Background color of form
        /// </summary>
        public ThemeColor FORM_BACKGROUND { get; set; }

        /// <summary>
        /// False for light based themes, true for dark based themes
        /// </summary>
        public bool DARK_BASED { get; set; }
    }
}
