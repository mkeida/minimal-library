using System;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Minimal.Themes;
using Minimal.External;
using Minimal.Properties;
using Minimal.Components.Core;
using System.Runtime.InteropServices;
using Minimal.Internal;

namespace Minimal
{
    /// <summary>
    /// Available types of the button
    /// </summary>
    public enum ButtonType { Raised, Flat, Outline }

    /// <summary>
    /// Available types of the label
    /// </summary>y
    public enum LabelType { Standard, Alternate, Accent }

    /// <summary>
    /// Available types of click effect
    /// </summary>
    public enum ClickEffect { None, Ripple, Square, SquareRotate }

    /// <summary>
    /// Available types of the color-box
    /// </summary>y
    public enum ColorBoxType { Rectangle, Circle }

    /// <summary>
    /// Simplifies working with the library
    /// </summary>
    public class M
    {
        /// <summary>
        /// List of all M-components on form
        /// </summary>
        public static List<IMComponent> IMComponents = new List<IMComponent>();

        /// <summary>
        /// Default light theme
        /// </summary>
        public static Theme Light { get; set; } = LoadTheme(Resources.light_theme);

        /// <summary>
        /// Default dark theme
        /// </summary>
        public static Theme Dark { get; set; } = LoadTheme(Resources.dark_theme);

        /// <summary>
        /// Application wide theme
        /// </summary>
        public static Theme ApplicationWideTheme = Light;

        /// <summary>
        /// Touch enabled private var
        /// </summary>
        private static bool _touchEnabled = false;

        /// <summary>
        /// TouchEnabled property. True if touch is enabled.
        /// </summary>
        public static bool TouchEnabled
        {
            get
            {
                // Return value
                return _touchEnabled;
            }

            set
            {
                // Update value
                _touchEnabled = value;

                // Iterate over all components
                foreach (IMComponent c in IMComponents)
                {
                    // Update input type
                    c.Component.InvokeInputTypeChanged(value);
                }
            }
        }

        [DllImport("dwmapi.dll", EntryPoint = "#127")]
        internal static extern void DwmGetColorizationParameters(ref DwmColorizationParams p);

        /// <summary>
        /// Initializes Minimal library. Must be called on every Form which don't
        /// inherits from MForm after InitializeComponent() method. 
        /// </summary>
        public static void EnableMinimal()
        {
            // Creates new timer
            Timer timer = new Timer
            {
                Enabled = true,
                Interval = 1
            };

            // Create update method
            timer.Tick += new EventHandler((object sender, EventArgs args) => 
            {
                // Iterate over all components
                foreach (IMComponent c in IMComponents)
                {
                    // Is component outdated?
                    if (c.Component.Outdated)
                        // Update component
                        c.Component.InvokeComponentUpdate();
                }
            });

            // Start timer
            timer.Start();
        }

        /// <summary>
        /// Load XML theme file and pass data to new Theme instance.
        /// </summary>
        public static Theme LoadTheme(string path)
        {
            // Initialize new Theme instance
            Theme theme = new Theme();

            // Try to load XML theme
            try
            {
                // Initialize new .xml document
                XmlDocument xmlDoc = new XmlDocument();

                // Try load from path
                try
                {
                    xmlDoc.Load(path);
                }
                catch
                {
                    // Try load from XML file
                    xmlDoc.LoadXml(path);
                }

                // Iterate through all possible ThemeColors
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    // Iterate through all ThemeColor's parameters and pass them to themeColor instance
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        // Theme color properties
                        try
                        {
                            // New ThemeColor instance
                            ThemeColor tc = new ThemeColor();

                            // Fills theme color
                            tc.Normal = new Hex(childNode.Attributes["Normal"].Value);
                            tc.Hover = new Hex(childNode.Attributes["Hover"].Value);
                            tc.Focus = new Hex(childNode.Attributes["Focus"].Value);
                            tc.Disabled = new Hex(childNode.Attributes["Disabled"].Value);

                            // Pass filled theme color instance to current theme property (node)
                            theme.GetType().GetProperty(node.Name).SetValue(theme, tc);
                        }
                        catch
                        {
                            // DARK BASED property
                            // Last theme property which is not ThemeColor child
                            theme.GetType().GetProperty(node.Name).SetValue(theme, Convert.ToBoolean(childNode.Attributes["Value"].Value));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Error -> probably bad XML theme file formating
                throw new Exception("Error in reading XML theme file. " + e.Message);
            }

            // Return initialized theme instance
            return theme;
        }

        /// <summary>
        /// Set theme to given form.
        /// </summary>
        public static void SetTheme(MForm form, Theme theme)
        {
            // Update theme
            form.Theme = theme;

            // Set form's background-color
            form.BackColor = theme.FORM_BACKGROUND.Normal;

            // Iterate through all M-components
            foreach (IMComponent c in IMComponents)
            {
                // Check if component in on target form
                if (c.Component.ParentForm == form)
                {
                    // Set theme
                    c.Component.SourceTheme = theme;

                    // Invoke theme changed event
                    c.Component.InvokeThemeChanged(theme);

                    // Force update
                    c.Component.Outdated = true;
                }
            }

            // Redraw form
            form.Invalidate(true);

            // Refresh form
            form.Refresh();
        }

        /// <summary>
        /// Set accent
        /// </summary>
        public static void SetAccent(Form form, Color color)
        {
            // Iterate through all M-components
            foreach (IMComponent c in IMComponents)
            {
                // Check if component in on target form
                if (c.Component.ParentForm == form)
                {
                    // Set component's accent color
                    c.Component.Accent = color;

                    // Update component
                    c.Component.Outdated = true;
                }
            }
        }

        /// <summary>
        /// Return Windows OS active color
        /// </summary>
        public static Color GetWindowsActiveColor()
        {
            // Create new DwmColorizationParams
            DwmColorizationParams p = new DwmColorizationParams();

            // Get CP
            DwmGetColorizationParameters(ref p);

            // Return active color
            return Color.FromArgb(
                (byte)(true ? 255 : p.ColorizationColor >> 24),
                (byte)(p.ColorizationColor >> 16),
                (byte)(p.ColorizationColor >> 8),
                (byte)p.ColorizationColor
            );
        }
    }
}
