using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using Minimal.External;
using Minimal.Internal;
using Minimal.Scaling;
using Minimal.Themes;
using Minimal.Components.Core;

namespace Minimal.Components
{
    /// <summary>
    /// Simple radio-button
    /// </summary>
    [Designer("Minimal.Components.Designer.RadioButtonDesigner")]
    public partial class MRadioButton : RadioButton, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        public MComponent Component { get; set; }

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
        /// Overrides auto-size
        /// </summary>
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = false; }
        }

        /// <summary>
        /// Current state of the control
        /// </summary>
        private bool _state;

        /// <summary>
        /// Tint alpha
        /// </summary>
        private byte _tintAlpha;

        /// <summary>
        /// Mouse position
        /// </summary>
        private Point _mouse;

        /// <summary>
        /// Radius of inner circle
        /// </summary>
        private double _radius;

        /// <summary>
        /// Hover alpha value
        /// </summary>
        private int _hoverAlpha;

        /// <summary>
        /// True if the mouse is inside a control
        /// </summary>
        private bool _hover;

        /// <summary>
        /// Constructor
        /// </summary>
        public MRadioButton()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initializes component
            InitializeComponent();

            // Default variables
            _state = false;
            _tintAlpha = 0;
            _radius = 0;
            _hoverAlpha = 0;
            _hover = false;
            DoubleBuffered = true;
            AutoSize = false;
            Height = DIP.Set(20);
            Font = new Font("Segoe UI", 7);
            Size = new Size(130, 21);

            // Redraw
            Invalidate();
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
        /// Update
        /// </summary>
        private void UpdateComponent(object sender, EventArgs e)
        {
            // Is radio-button checked?
            if (_state)
            {
                // Radius
                if (_radius < 5)
                    _radius += 0.35;

                // Alpha
                if (_tintAlpha < 255)
                    _tintAlpha += 15;
            }
            else
            {
                // Radius
                if (_radius > 0)
                    _radius -= 0.35;

                // Alpha
                if (_tintAlpha > 0)
                    _tintAlpha -= 15;
            }

            // Hover
            if (_hover)
            {
                // Increase hover alpha value
                if (_hoverAlpha < 255)
                    _hoverAlpha += 15;
            }
            else
            {
                // Decrease hover alpha value
                if (_hoverAlpha > 0)
                    _hoverAlpha -= 15;
            }

            // Turn off updates
            if (!_hover && _hoverAlpha == 0 && _tintAlpha == 0)
                Component.Outdated = false;

            // Call paint method
            Invalidate();
        }

        /// <summary>
        /// Draw
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base call
            base.OnPaint(e);

            // Graphics
            Graphics g = e.Graphics;

            // Clear control
            g.Clear(Parent.BackColor);

            // Fill color, text and border
            Color fill = new Color();
            Color border = new Color();
            Color hover = new Color();
            Color foreground = new Color();

            if (Enabled)
            {
                // Enabled
                fill = Component.SourceTheme.COMPONENT_BACKGROUND.Normal.ToColor();
                border = Component.SourceTheme.COMPONENT_BORDER.Normal.ToColor();
                foreground = Component.SourceTheme.COMPONENT_FOREGROUND.Normal.ToColor();
                hover = Component.SourceTheme.COMPONENT_FILL.Hover.ToColor();
            }
            else
            {
                // Disabled
                fill = Component.SourceTheme.COMPONENT_BACKGROUND.Disabled.ToColor();
                border = Component.SourceTheme.COMPONENT_BORDER.Disabled.ToColor();
                foreground = Component.SourceTheme.COMPONENT_FOREGROUND.Disabled.ToColor();
                hover = Component.SourceTheme.COMPONENT_FILL.Disabled.ToColor();
            }

            // Draw background
            using (SolidBrush brush = new SolidBrush(fill))
                g.FillPath(brush, Draw.GetEllipsePath(new Point(DIP.Set(10), DIP.Set(10)), DIP.Set((10))));

            // Anti-aliasing
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw border
            if (!_state)
            {
                using (Pen pen = new Pen(!Checked ? Component.SourceTheme.COMPONENT_BORDER.Normal.ToColor() : Component.Accent, 1))
                    g.DrawPath(pen, Draw.GetEllipsePath(new Point(DIP.Set(10), DIP.Set(10)), DIP.Set(10)));
            }

            // Draw mouse hover effect
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(_hoverAlpha, hover)))
                g.FillPath(brush, Draw.GetEllipsePath(new Point(DIP.Set(10), DIP.Set(10)), DIP.Set(3)));

            // Animation - inner circle
            using (SolidBrush brush = new SolidBrush(Enabled ? Component.Accent : Component.SourceTheme.COMPONENT_FILL.Disabled.ToColor()))
                g.FillPath(brush, Draw.GetEllipsePath(new Point(DIP.Set(10), DIP.Set(10)), Convert.ToInt32(_radius)));

            // Animation - border tint
            using (Pen pen = new Pen(ColorExtensions.Mix(Color.FromArgb(_tintAlpha, Enabled ? Component.Accent : Component.SourceTheme.COMPONENT_BORDER.Disabled.ToColor()), border)))
                g.DrawEllipse(pen, new Rectangle(0, 0, DIP.Set(20), DIP.Set(20)));

            // Anti-aliasing
            g.SmoothingMode = SmoothingMode.Default;

            // Draw text
            using (SolidBrush brush = new SolidBrush(foreground))
                g.DrawString(Text, Font, brush, new Point(25 + DIP.Set(8), 2));
        }

        /// <summary>
        /// Checked changed event
        /// </summary>
        protected override void OnCheckedChanged(EventArgs e)
        {
            // Base call
            base.OnCheckedChanged(e);

            // Update state
            _state = Checked;
        }

        /// <summary>
        /// Mouse move
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Base call
            base.OnMouseDown(e);

            // Update mouse position
            _mouse = new Point(e.X, e.Y);
        }

        /// <summary>
        /// OnMouseEnter method. Check if mouse is inside of control
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            // Base call
            base.OnMouseEnter(e);

            // Update hover
            _hover = true;

            // Turn on updates
            Component.Outdated = true;
        }

        /// <summary>
        /// OnMouseLeave method. Check if mouse is outside of control
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            // Base call
            base.OnMouseLeave(e);

            // Update hover
            _hover = false;
        }
    }
}
