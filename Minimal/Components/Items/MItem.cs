using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading.Tasks;
using Minimal.External;
using Minimal.Internal;
using Minimal.Scaling;

namespace Minimal.Components.Items
{
    /// <summary>
    /// Basic item class.
    /// </summary>
    public class MItem
    { 
        /// <summary>
        /// Owner of item
        /// </summary>
        public MListBox Owner { get; set; }

        /// <summary>
        /// Primary text property
        /// </summary>
        public string PrimaryText { get; set; }

        /// <summary>
        /// Bounds property
        /// </summary>
        internal Rectangle Bounds { get; set; }

        /// <summary>
        /// Height property
        /// </summary>
        internal int Height { get; set; }

        /// <summary>
        /// True if mouse hovers over an item
        /// </summary>
        public bool Hover { get; set; }

        /// <summary>
        /// True if an item is selected
        /// </summary>
        public bool Selected
        {
            get
            {
                return Owner.SelectedItem == this || (Owner.SelectedItems.Contains(this) && Owner.MultiSelect);
            }
        }

        /// <summary>
        /// If true, item acts like a divider
        /// </summary>
        public bool Divider { get; set; } = false;

        /// <summary>
        /// Click effect of an item
        /// </summary>
        public ClickEffect ClickEffect { get; set; } = ClickEffect.Ripple;

        /// <summary>
        /// Alpha value of hover effect
        /// </summary>
        private int _hoverAlpha = 0;

        /// <summary>
        /// Length of the diagonal of an item
        /// </summary>
        private int _diagonal;

        /// <summary>
        /// Radius of the click effect
        /// </summary>
        private double _radius;

        /// <summary>
        /// Radius increment of the click effect
        /// </summary>
        private double _increment = 1;

        /// <summary>
        /// Rotation of the click effect (in degrees)
        /// </summary>
        private int _rotation;

        /// <summary>
        /// Alpha value of the click effect
        /// </summary>
        private byte _clickAlpha;

        /// <summary>
        /// Point where user clicked relative to this item
        /// </summary>
        private Point _mouseClickPoint = new Point();

        /// <summary>
        /// Mouse position
        /// </summary>
        private Point _mousePosition = new Point();

        /// <summary>
        /// Constructor
        /// </summary>
        public MItem()
        {
            // Default item height
            // Uses device independent pixels to provide best scaling
            // experience
            Height = DIP.Set(50);
        }

        /// <summary>
        /// Update method
        /// </summary>
        protected virtual void OnUpdate(object sender, EventArgs e)
        {
            // If item is not in the view-port
            if (!ItemInView(this))
            {
                // Reset animation variables
                _hoverAlpha = 0;
                _radius = 0;
                _rotation = 0;
                _clickAlpha = 0;

                // Go back
                return;
            }

            // Updates background alpha value of
            // hover effect
            if (Hover)
            {
                if (_hoverAlpha < 30)
                    _hoverAlpha += 2;
            }
            else
            {
                if (_hoverAlpha > 0)
                    _hoverAlpha -= 2;
            }

            // Update diagonal
            _diagonal = (int)(2 * Math.Sqrt(Math.Pow(Bounds.Width, 2) + Math.Pow(Bounds.Height, 2)));

            // ClickEffect
            // If radius is smaller than half of diagonal length, then increase radius by ink inkIncrement
            if (_radius < _diagonal / 2)
                _radius += _increment;

            // Decrease alpha if it's not zero else reset animation variables
            if (_clickAlpha > 0)
                // Decrement alpha
                _clickAlpha -= 1;
            else
            {
                // Restart animation variables
                _radius = 0;
                _clickAlpha = 0;
            }

            // Update rotation
            if (_rotation < 360)
                _rotation++;
            else
                _rotation = 0;
        }

        /// <summary>
        /// Draw item method
        /// </summary>
        protected virtual void OnDrawItem(Graphics g, Rectangle bounds)
        {
            if (Owner == null)
                return;

            // If item is not in the view-port
            if (!ItemInView(this))
            {
                // Go back
                return;
            }

            // Is item divider?
            if (Divider)
            {
                // Draws background
                using (SolidBrush brush = new SolidBrush(Owner.Component.SourceTheme.COMPONENT_BACKGROUND.Normal))
                    g.FillRectangle(brush, Bounds);

                // End
                return;
            }

            // Updates item's height property
            // Uses device independent pixels to provide best scaling
            // experience
            Height = DIP.Set(50);

            // Update bounds property
            Bounds = bounds;

            // Set clip region. Clip graphics canvas to the dimensions of
            // list item.
            g.Clip = new Region(bounds);

            // Draws background
            using (SolidBrush brush = new SolidBrush(Owner.Component.SourceTheme.COMPONENT_BACKGROUND.Normal))
                g.FillRectangle(brush, Bounds);

            // If touch-controls are disabled
            if (!M.TouchEnabled)
            {
                // Shortcut variable for normal control background color
                Color bg = Owner.Component.SourceTheme.COMPONENT_BACKGROUND.Normal;

                // Draws partially transparent accent layer as background
                using (SolidBrush brush = new SolidBrush(ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, Owner.Accent), bg)))
                    g.FillRectangle(brush, Bounds);

                // Turn on anti-aliasing
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Reveal effect ellipse
                GraphicsPath ellipse = Draw.GetEllipsePath(_mousePosition, 150);

                // Path gradient brush
                PathGradientBrush pgb = new PathGradientBrush(ellipse);
                pgb.CenterPoint = _mousePosition;
                pgb.CenterColor = Color.FromArgb(55, Owner.Accent);
                pgb.SurroundColors = new Color[] { ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, Owner.Accent), bg) };

                // Draw reveal effect if mouse is hovering over an item
                if (Hover)
                    g.FillPath(pgb, ellipse);

                // Turn off anti-aliasing
                g.SmoothingMode = SmoothingMode.Default;

                // Draws partially transparent accent layer as overlay
                // to hide rest of reveal ellipse
                using (SolidBrush brush = new SolidBrush(ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, Owner.Accent), bg)))
                    g.FillRectangle(brush, new Rectangle(Bounds.X + 2, Bounds.Y + 2, Bounds.Width - 4, Bounds.Height - 4));
            }

            // Draws click effect
            DrawClick(g);
        }

        /// <summary>
        /// Mouse click
        /// </summary>
        protected virtual void OnMouseClick(object sender, MouseEventArgs e)
        {
            // Dis user left-clicked?
            if (e.Button == MouseButtons.Left)
            {
                // Update mouse click point
                _mouseClickPoint = e.Location;

                // Updates click-effects
                // Ink
                if (ClickEffect == ClickEffect.Ripple)
                    _radius = Bounds.Width / 5;

                // Square
                if (ClickEffect == ClickEffect.Square || ClickEffect == ClickEffect.SquareRotate)
                    _radius = Bounds.Width / 8;

                // Resets alpha
                _clickAlpha = 25;
            }
        }

        /// <summary>
        /// Mouse enter
        /// </summary>
        protected virtual void OnMouseEnter(object sender, MouseEventArgs e)
        {
            // Update hover
            Hover = true;
        }

        /// <summary>
        /// Mouse leave
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseLeave(object sender, MouseEventArgs e)
        {
            // Update hover
            Hover = false;
        }

        /// <summary>
        /// Mouse move
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseMove(object sender, MouseEventArgs e)
        {
            // Update mouse position
            _mousePosition = e.Location;
        }

        /// <summary>
        /// Calls draw item method
        /// </summary>
        internal void TriggerDrawItem(Graphics g, Rectangle itemBounds)
        {
            OnDrawItem(g, itemBounds);
        }

        /// <summary>
        /// Triggers mouse-click event
        /// </summary>
        internal void TriggerUpdate(object sender, EventArgs e)
        {
            OnUpdate(sender, e);
        }

        /// <summary>
        /// Triggers mouse-click event
        /// </summary>
        internal void TriggerMouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(sender, e);
        }

        /// <summary>
        /// Triggers mouse-enter event
        /// </summary>
        internal void TriggerMouseEnter(object sender, MouseEventArgs e)
        {
            OnMouseEnter(sender, e);
        }

        /// <summary>
        /// Triggers mouse-leave event
        /// </summary>
        internal void TriggerMouseLeave(object sender, MouseEventArgs e)
        {
            OnMouseLeave(sender, e);
        }

        /// <summary>
        /// Triggers mouse-move event
        /// </summary>
        internal void TriggerMouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(sender, e);
        }

        /// <summary>
        /// Draw click effect
        /// </summary>
        private void DrawClick(Graphics g)
        {
            // ClickEffect
            if (ClickEffect != ClickEffect.None)
            {
                // Color of click effect
                Color color;
                Color fill = Owner.Accent;

                if (Owner.Component.SourceTheme.DARK_BASED == true)
                {
                    // Dark based themes
                    color = Color.FromArgb(_clickAlpha, ColorExtensions.AddRGB(150, fill));
                }
                else
                {
                    // Light based themes
                    color = Color.FromArgb(_clickAlpha, ColorExtensions.AddRGB(-150, fill));
                }

                // Draws click effect
                // Set up anti-aliasing
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Ripple
                if (ClickEffect == ClickEffect.Ripple)
                {
                    // Ink's brush and graphics path
                    GraphicsPath ink = Draw.GetEllipsePath(_mouseClickPoint, (int)_radius);

                    // Draws ripple click effect
                    using (SolidBrush brush = new SolidBrush(color))
                        g.FillPath(brush, ink);
                }

                // Square
                if (ClickEffect == ClickEffect.Square || ClickEffect == ClickEffect.SquareRotate)
                {
                    // Square's brush and graphics path
                    GraphicsPath square = Draw.GetSquarePath(_mouseClickPoint, (int)_radius);

                    // Rotates square
                    if (ClickEffect == ClickEffect.SquareRotate)
                    {
                        Matrix matrix = new Matrix();
                        matrix.RotateAt(_rotation, _mouseClickPoint);
                        square.Transform(matrix);
                    }

                    // Draws square click effect
                    using (SolidBrush brush = new SolidBrush(color))
                        g.FillPath(brush, square);
                }

                // Remove anti-aliasing
                g.SmoothingMode = SmoothingMode.Default;
            }
        }

        /// <summary>
        /// Returns true of false depending on if an given item
        /// is in visible view-port
        /// </summary>
        private bool ItemInView(MItem item)
        {
            // Check for top
            if (item.Bounds.Y + item.Bounds.Height > Math.Abs(Owner.Panel.AutoScrollPosition.Y))
            {
                // Check for bottom
                if (item.Bounds.Y < Math.Abs(Owner.Panel.AutoScrollPosition.Y) + Owner.Height)
                {
                    return true;
                }
            }

            // Not in view port
            return false;
        }

        /// <summary>
        /// Implicit operator for String conversion
        /// </summary>
        public static implicit operator MItem(string str)
        {
            return new SingleLineItem(str);
        }
    }
}
