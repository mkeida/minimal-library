using Minimal.External;
using Minimal.Internal;
using Minimal.Scaling;
using System;
using System.Drawing;

namespace Minimal.Components.Items
{
    /// <summary>
    /// Two line item
    /// </summary>
    public class TwoLineItem : MItem
    {
        /// <summary>
        /// Secondary text
        /// </summary>
        public string SecondaryText { get; set; }

        /// <summary>
        /// Alignment of primary text
        /// </summary>
        ContentAlignment TextAlign { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TwoLineItem(string primaryText, string secondaryText, ContentAlignment textAlign = ContentAlignment.MiddleLeft)
        {
            // Primary text
            PrimaryText = primaryText;

            // Secondary text
            SecondaryText = secondaryText;

            // Alignment
            TextAlign = textAlign;

            // Default height
            Height = DIP.Set(70);
        }

        /// <summary>
        /// Draw item
        /// </summary>
        protected override void OnDrawItem(Graphics g, Rectangle bounds)
        {
            // Base call
            base.OnDrawItem(g, bounds);

            // Default height
            Height = DIP.Set(70);

            // String format
            // Converts alignment types
            StringFormat format = new StringFormat();
            int lNum = (int)Math.Log((double)TextAlign, 2);
            format.LineAlignment = (StringAlignment)(lNum / 4);
            format.Alignment = (StringAlignment)(lNum % 4);

            // Creates color of secondary text
            Color color = (Owner.Component.SourceTheme.DARK_BASED) ? ColorExtensions.AddRGB(-120, Owner.Component.SourceTheme.COMPONENT_FOREGROUND.Normal) : ColorExtensions.AddRGB(100, Owner.Component.SourceTheme.COMPONENT_FOREGROUND.Normal);

            // Draws secondary text
            using (SolidBrush brush = new SolidBrush(color))
                g.DrawString(SecondaryText, new Font("Segoe UI", 8), brush, new Rectangle(Bounds.X + 10, Bounds.Y + 12, Bounds.Width - 20, Bounds.Height), format);

            // Draws primary text
            using (SolidBrush brush = new SolidBrush(Selected ? Owner.Accent : Owner.Component.SourceTheme.COMPONENT_FOREGROUND.Normal.ToColor()))
                g.DrawString(PrimaryText, new Font("Segoe UI", 9), brush, new Rectangle(Bounds.X + 10, Bounds.Y - 12, Bounds.Width - 20, Bounds.Height), format);
        }
    }
}
