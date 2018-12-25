using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Minimal.External;
using Minimal.Internal;
using Minimal.Themes;
using Minimal.Components.Core;

namespace Minimal.Components
{
    /// <summary>
    /// Combo-box. Upper part of control.
    /// </summary>
    [ToolboxItem(false)]
    public partial class MComboBoxHeader : UserControl, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        public MComponent Component { get; set; }

        /// <summary>
        /// Default click effect
        /// </summary>
        private ClickEffect _clickEffect = ClickEffect.Ripple;

        /// <summary>
        /// Button ClickEffect changed event handler
        /// </summary>
        public event PropertyChangedEventHandler ClickEffectChanged;

        /// <summary>
        /// ClickEffect property
        /// </summary>
        public ClickEffect ClickEffect
        {
            get { return _clickEffect; }
            set
            {
                if (value != _clickEffect)
                {
                    _clickEffect = value;
                    ClickEffectChanged?.Invoke(this, new PropertyChangedEventArgs("ClickEffectChanged"));
                }

                // Calls paint method
                Invalidate();
            }
        }

        /// <summary>
        /// Accent changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the Accent property is changed.")]
        public event PropertyChangedEventHandler AccentChanged;

        /// <summary>
        /// Accent property
        /// </summary>
        [Category("Minimal")]
        [Description("Component's accent color. Main visible color of the control.")]
        public Color Accent
        {
            get { return Component.Accent; }
            set
            {
                if (value != Component.Accent)
                {
                    // Update actual accent value
                    Component.Accent = value;

                    // Invoke property changed event handler
                    AccentChanged?.Invoke(this, new PropertyChangedEventArgs("AccentChanged"));

                    // Redraw component
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Custom theme changed event
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the CustomTheme property is changed.")]
        public event PropertyChangedEventHandler CustomThemeChanged;

        /// <summary>
        /// Custom theme property
        /// </summary>
        [Category("Minimal")]
        [Description("Component's custom theme. Will override default parent M-form or application-wide theme.")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Theme CustomTheme
        {
            get { return Component.CustomTheme; }
            set
            {
                // Change custom theme
                Component.CustomTheme = value;

                // Invoke event
                CustomThemeChanged?.Invoke(this, new PropertyChangedEventArgs("CustomThemeChanged"));
            }
        }

        /// <summary>
        /// Alpha value of the hover effect which is later added to control fill color
        /// </summary>
        private byte _hoverAlpha;

        /// <summary>
        /// Mouse position relative to the control's top left corner
        /// </summary>
        private Point _mouse;

        /// <summary>
        /// Alpha value of the ClickEffect
        /// </summary>
        private byte _alpha;

        /// <summary>
        /// Length of the button diagonal
        /// </summary>
        private int _diagonal;

        /// <summary>
        /// Radius of the ClickEffect
        /// </summary>
        private double _radius;

        /// <summary>
        /// Radius increment of the ClickEffect
        /// </summary>
        private double _increment;

        /// <summary>
        /// Rotation of the ClickEffect in degrees
        /// </summary>
        private int _rotation;

        /// <summary>
        /// True if the mouse is inside of the control
        /// </summary>
        internal bool hover;

        /// <summary>
        /// Constructor
        /// </summary>
        public MComboBoxHeader()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialize
            InitializeComponent();

            // Default variables and properties
            _increment = 1;
            DoubleBuffered = true;
            Text = "ComboBox";
        }

        /// <summary>
        /// Occurs when a handle is created for the control. Handles
        /// event hooking.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            // Base call
            base.OnHandleCreated(e);

            // Hook up update method
            Component.ComponentUpdate += UpdateComponent;
        }

        /// <summary>
        /// Update method
        /// </summary>
        private void UpdateComponent(object sender, EventArgs e)
        {
            // Hover effect
            if (hover)
            {
                if (_hoverAlpha < 255)
                    _hoverAlpha += 15;
            }
            else
            {
                if (_hoverAlpha > 0)
                    _hoverAlpha -= 15;
            }

            // ClickEffect
            // If radius is smaller than half of diagonal length, then increase radius by ink inkIncrement
            if (_radius < _diagonal / 2)
                _radius += _increment;

            // Decrease alpha if it's not zero else reset animation variables
            if (_alpha > 0)
                _alpha -= 1;
            else
            { 
                _radius = 0;
                _alpha = 0;
            }

            // Rotation
            if (_rotation < 360)
                _rotation++;
            else
                _rotation = 0;

            // Turn off timer
            if (!hover && _radius == 0 && _alpha == 0 && _hoverAlpha == 0)
                Component.Outdated = false;

            // Redraw
            Invalidate();
        }

        /// <summary>
        /// Draw method
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base call
            base.OnPaint(e);

            // Get graphics context
            Graphics g = e.Graphics;

            // Clear control
            g.Clear(Parent.BackColor);

            // Regular raised button
            Color fill = ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, Component.SourceTheme.COMPONENT_FILL.Hover), Component.SourceTheme.COMPONENT_FILL.Normal);
            Color text = Component.SourceTheme.COMPONENT_FOREGROUND.Normal.ToColor();

            // Disabled
            if (!Enabled)
            {
                fill = Component.SourceTheme.COMPONENT_FILL.Disabled.ToColor();
                text = Component.SourceTheme.COMPONENT_FOREGROUND.Disabled.ToColor();
            }

            // Fill control
            using (SolidBrush brush = new SolidBrush(fill))
                g.FillRectangle(brush, ClientRectangle);

            // Click effect
            DrawClick(e);

            // Format text
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;

            // Draw text
            using (SolidBrush brush = new SolidBrush(text))
                g.DrawString(Text, new Font("Segoe UI", 9), brush, new Rectangle(ClientRectangle.X + 10, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height), sf);
        }

        /// <summary>
        /// Draw click effect
        /// </summary>
        /// <param name="e"></param>
        private void DrawClick(PaintEventArgs e)
        {
            // Control's graphics object
            Graphics g = e.Graphics;

            // ClickEffect
            if (_clickEffect != ClickEffect.None)
            {
                // Color of ClickEffect
                Color color;
                Color fill = Component.SourceTheme.COMPONENT_FILL.Normal.ToColor();

                if (Component.SourceTheme.DARK_BASED == true)
                    // Dark based themes
                    color = Color.FromArgb(_alpha, ColorExtensions.AddRGB(150, fill));
                else
                    // Light based themes
                    color = Color.FromArgb(_alpha, ColorExtensions.AddRGB(-150, fill));

                // Draws ClickEffect
                // Set up anti-aliasing
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Ink
                if (_clickEffect == ClickEffect.Ripple)
                {
                    // Ink's brush and graphics path
                    GraphicsPath ink = Draw.GetEllipsePath(_mouse, (int)_radius);

                    // Draws ink ClickEffect
                    using (SolidBrush brush = new SolidBrush(color))
                        g.FillPath(brush, ink);
                }

                // Square
                if (_clickEffect == ClickEffect.Square || _clickEffect == ClickEffect.SquareRotate)
                {
                    // Square's brush and graphics path
                    GraphicsPath square = Draw.GetSquarePath(_mouse, (int)_radius);

                    // Rotates square
                    if (_clickEffect == ClickEffect.SquareRotate)
                    {
                        Matrix matrix = new Matrix();
                        matrix.RotateAt(_rotation, _mouse);
                        square.Transform(matrix);
                    }

                    // Draws square ClickEffect
                    using (SolidBrush brush = new SolidBrush(color))
                        g.FillPath(brush, square);
                }

                // Remove anti-aliasing
                g.SmoothingMode = SmoothingMode.Default;
            }
        }

        /// <summary>
        /// OnMouseEnter method. Check if mouse is inside of button
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            // Base call
            base.OnMouseEnter(e);

            // Update hover variable
            hover = true;

            // Turn on timer
            Component.Outdated = true;
        }

        /// <summary>
        /// OnMouseLeave method. Check if mouse is outside of button
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            // Base call
            base.OnMouseLeave(e);

            // Update hover variable
            hover = false;
        }

        /// <summary>
        /// Control resize event
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            // Base call
            base.OnResize(e);

            // Update diagonal variable length
            _diagonal = (int)(2 * Math.Sqrt(Math.Pow(Width, 2) + Math.Pow(Height, 2)));
        }

        /// <summary>
        /// Control click event
        /// </summary>
        protected override void OnClick(EventArgs e)
        {
            // Base call
            base.OnClick(e);

            // Set up ClickEffect variables
            _mouse = PointToClient(MousePosition);

            // ClickEffect
            // Ink
            if (_clickEffect == ClickEffect.Ripple)
                _radius = Width / 5;

            // Square
            if (_clickEffect == ClickEffect.Square || _clickEffect == ClickEffect.SquareRotate)
                _radius = Width / 8;

            // Resets alpha
            _alpha = 25;
        }

        /// <summary>
        /// On got focus
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e)
        {
            // Base call
            base.OnGotFocus(e);

            // Control needs to be updated
            Component.Outdated = true;
        }
    }
}
