using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minimal.Components.Items
{
    /// <summary>
    /// Single line item
    /// </summary>
    public class SingleLineItem : MItem
    {
        /// <summary>
        /// Alignment of primary text
        /// </summary>
        ContentAlignment TextAlign { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SingleLineItem(string primaryText, ContentAlignment textAlign = ContentAlignment.MiddleLeft)
        {
            // Primary text
            PrimaryText = primaryText;

            // Alignment
            TextAlign = textAlign;
        }

        /// <summary>
        /// Draw item
        /// </summary>
        protected override void OnDrawItem(Graphics g, Rectangle bounds)
        {
            // Base call
            base.OnDrawItem(g, bounds);

            // String format
            // Converts alignment types
            StringFormat format = new StringFormat();
            int lNum = (int)Math.Log((double)TextAlign, 2);
            format.LineAlignment = (StringAlignment)(lNum / 4);
            format.Alignment = (StringAlignment)(lNum % 4);

            // Draws primary text
            using (SolidBrush brush = new SolidBrush(Selected ? Owner.Accent : Owner.Component.SourceTheme.COMPONENT_FOREGROUND.Normal.ToColor()))
                g.DrawString(PrimaryText, new Font("Segoe UI", 9), brush, new Rectangle(Bounds.X + 10, Bounds.Y, Bounds.Width - 20, Bounds.Height), format);
        }
    }
}
