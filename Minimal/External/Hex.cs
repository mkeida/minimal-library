using System.Drawing;

namespace Minimal.External
{
    /// <summary>
    /// Provides support for HEX colors.
    /// </summary>
    public class Hex
    {
        /// <summary>
        /// Value of hex
        /// </summary>
        private string value;

        /// <summary>
        /// Value property. Returns color in HEX format.
        /// </summary>
        public string Value
        {
            get { return value; }
            set { value = this.value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Hex(string hex)
        {
            // Validation
            if (!Validate(hex))
            {
                throw new System.FormatException("Invalid hex code format!");
            }

            // No hex code specified
            if (hex.Length == 0)
            {
                // Set black by default
                value = "#000000FF";
            }

            // Hex with alpha value
            if (hex.Length == 9)
            {
                value = hex;
            }

            // Hex without alpha value
            if (hex.Length == 7)
            {
                // Automatically add FF (255) to alpha value
                value = hex + "FF";
            }
        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        public Hex(Color color)
        {
            value = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") + color.A.ToString("X2");
        }

        /// <summary>
        /// Converts given HEX color to System.Drawing.Color
        /// </summary>
        /// <returns>Converted HEX color</returns>
        public Color ToColor()
        {
            // Get alpha part - last two numbers
            string alpha = value.Substring(value.Length - 2);

            // Convert only first part (without alpha value)
            Color color = ColorTranslator.FromHtml(value.Remove(value.Length - 2, 2));

            // Convert last part (only alpha value)
            int alphaValue = int.Parse(alpha, System.Globalization.NumberStyles.HexNumber);

            // Add alpha value to color
            return Color.FromArgb(alphaValue, color);
        }

        /// <summary>
        /// Implicit operator for Color conversion
        /// </summary>
        public static implicit operator Color(Hex hex)
        {
            return hex.ToColor();
        }

        /// <summary>
        /// Implicit operator for Hex conversion
        /// </summary>
        public static implicit operator Hex(Color color)
        {
            return new Hex(color);
        }

        /// <summary>
        /// Implicit operator for String conversion
        /// </summary>
        public static implicit operator Hex(string str)
        {
            return new Hex(str);
        }

        /// <summary>
        /// Validates if string is also a valid hex code
        /// </summary>
        /// <param name="code">String to validate</param>
        /// <returns>True if string is also a valid hex color code</returns>
        public static bool Validate(string code)
        {
            // Validates hex code
            return System.Text.RegularExpressions.Regex.IsMatch(code, @"^#([0-9A-F]{6}|[0-9A-F]{8})$");
        }

        // Predefined colors
        public static Hex red = new Hex("#E53935");
        public static Hex pink = new Hex("#D81B60");
        public static Hex purple = new Hex("#8E24AA");
        public static Hex deepPurple = new Hex("#5E35B1");
        public static Hex indigo = new Hex("#3949AB");
        public static Hex blue = new Hex("#1E88E5");
        public static Hex seaBlue = new Hex("#304FFE");
        public static Hex lightBlue = new Hex("#039BE5");
        public static Hex cyan = new Hex("#00ACC1");
        public static Hex teal = new Hex("#00897B");
        public static Hex green = new Hex("#43A047");
        public static Hex lightGreen = new Hex("#7CB342");
        public static Hex lime = new Hex("#C0CA33");
        public static Hex yellow = new Hex("#FDD835");
        public static Hex amber = new Hex("#FFB300");
        public static Hex orange = new Hex("#FB8C00");
        public static Hex deepOrange = new Hex("#F4511E");
        public static Hex brown = new Hex("#6D4C41");
        public static Hex grey = new Hex("#757575");
        public static Hex blueGray = new Hex("#546E7A");
        public static Hex black = new Hex("#000000");
        public static Hex white = new Hex("#FFFFFF");
    }
}
