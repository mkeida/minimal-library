using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using Minimal.Themes;
using Minimal.Internal;
using Minimal.External;
using Minimal.Scaling;
using Minimal.Components.Core;

namespace Minimal.Components
{
    /// <summary>
    /// Check-box control
    /// </summary>
    [Designer("Minimal.Components.Designer.CheckBoxDesigner")]
    public partial class MCheckBox : CheckBox, IMComponent
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
        [Description("Component's accent color. Main visible color of the component.")]
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
        /// Auto-size. Always false. Allow to set custom height of check box.
        /// </summary>
        public override bool AutoSize
        {
            set { base.AutoSize = false; }
            get { return base.AutoSize; }
        }

        /// <summary>
        /// Hides default Checked property
        /// </summary>
        public new bool Checked
        {
            set
            {
                // Update checked
                base.Checked = value;

                // Redraw
                Component.Outdated = true;
            }

            get { return base.Checked; }
        }

        /// <summary>
        /// Current state of the control
        /// </summary>
        private bool _state;

        /// <summary>
        /// Size of the cover square
        /// </summary>
        private int _a;

        /// <summary>
        /// Handles check-mark animation progress
        /// </summary>
        private int _frame;

        /// <summary>
        /// Check-mark point array
        /// </summary>
        private Point[] _checkmark = { };

        /// <summary>
        /// True if the mouse is inside of the control
        /// </summary>
        private bool _hover;

        /// <summary>
        /// Alpha value of the hover effect which is later added to control fill color
        /// </summary>
        private byte _hoverAlpha;

        /// <summary>
        /// Constructor
        /// </summary>
        public MCheckBox()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialize
            InitializeComponent();

            // Default variables
            _state = false;
            _a = 0;
            _frame = 0;
            _hover = false;
            _hoverAlpha = 0;
            DoubleBuffered = true;
            AutoSize = false;
            Height = DIP.Set(15);
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
        /// Update method
        /// </summary>
        private void UpdateComponent(object sender, EventArgs e)
        {
            // Animation variables
            if (_state)
            {
                if (_a < 10)
                    _a++;
            }
            else
            {
                if (_frame == _checkmark.Length)
                    if (_a > 0)
                        _a--;
            }

            // Frame of the check-mark animation
            if (_frame < _checkmark.Length)
            {
                if (_a == 0 || _a == 10)
                    _frame++;
            }
            else
                _frame = _checkmark.Length;

            // Hover effect
            if (_hover)
            {
                if (_hoverAlpha < 255)
                    _hoverAlpha += 15;
            }
            else
            {
                if (_hoverAlpha > 0)
                    _hoverAlpha -= 15;
            }

            // Turn off timer
            if (!_hover && _hoverAlpha == 0 && (_a == 0 || _a == 10) && (_frame == _checkmark.Length || _frame == 0))
                Component.Outdated = true;

            // Redraw control
            Invalidate();
        }

        /// <summary>
        /// Draws entire check-box
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base painting
            base.OnPaint(e);

            // Get graphics context
            Graphics g = e.Graphics;

            // Clear control
            g.Clear(Parent.BackColor);

            // Fill color, text and border
            Color fill = new Color();
            Color border = new Color();
            Color foreground = new Color();

            // Handle colors
            if (Enabled)
            {
                // Enabled
                fill = ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, Component.SourceTheme.COMPONENT_BACKGROUND.Hover.ToColor()), Component.SourceTheme.COMPONENT_BACKGROUND.Normal.ToColor());
                border = (Focused) ? Accent : Component.SourceTheme.COMPONENT_BORDER.Normal.ToColor();
                foreground = Component.SourceTheme.COMPONENT_FOREGROUND.Normal.ToColor();
            }
            else
            {
                // Disabled
                fill = Component.SourceTheme.COMPONENT_BACKGROUND.Disabled.ToColor();
                border = Component.SourceTheme.COMPONENT_BORDER.Disabled.ToColor();
                foreground = Component.SourceTheme.COMPONENT_FOREGROUND.Disabled.ToColor();
            }

            // Check-box's height
            int height = 19;

            // Draw unchecked check-box
            using (SolidBrush brush = new SolidBrush(fill))
                g.FillRectangle(brush, new Rectangle(0, Height / 2 - height / 2, height, height));

            // Draw check-box's border
            using (Pen pen = new Pen(border))
                g.DrawRectangle(pen, new Rectangle(0, Height / 2 - height / 2, height, height));

            // Get cover rectangle (square)
            GraphicsPath square = Draw.GetSquarePath(new Point(10, Height / 2 - height / 2 + 10), _a);

            // Draw cover rectangle (square)
            using (SolidBrush brush = new SolidBrush(Enabled ? Component.Accent : Component.SourceTheme.COMPONENT_FILL.Disabled.ToColor()))
                g.FillPath(brush, square);

            // Middle of check-box
            int b = Height / 2 - height / 2;

            // Check-mark points
            _checkmark = new Point[]
            {
                new Point(4, b + 9),
                new Point(5, b + 10),
                new Point(6, b + 11),
                new Point(7, b + 12),
                new Point(8, b + 13),
                new Point(9, b + 12),
                new Point(10, b + 11),
                new Point(11, b + 10),
                new Point(12, b + 9),
                new Point(13, b + 8),
                new Point(14, b + 7),
                new Point(15, b + 6),
            };

            // Draws check-mark in animation
            if (_a == 10 && _state == true)
            {
                // Draws two check-marks with 1 width of pen. Setting pen's
                // width to 2 pixels causes one missing pixel at bottom of the
                // check-mark.
                DrawCheckmarkIn(g);
                DrawCheckmarkIn(g, 1, 1);
            }

            // Draws check-mark out animation
            if (_a == 10 && _state == false)
            {
                // Draws two check-marks with 1 width of pen. Setting pen's
                // width to 2 pixels causes one missing pixel at bottom of the
                // check-mark.
                DrawCheckmarkOut(g);
                DrawCheckmarkOut(g, 1, 1);
            }

            // Draws text
            using (SolidBrush brush = new SolidBrush(foreground))
                g.DrawString(this.Text, this.Font, brush, new Point(25, b));
        }

        /// <summary>
        /// OnMouseEnter method. Check if mouse is inside of button
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            // Base call
            base.OnMouseEnter(e);

            // Update hover variable
            _hover = true;

            // Control needs to be updated
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
            _hover = false;
        }

        /// <summary>
        /// Checked changed event
        /// </summary>
        protected override void OnCheckedChanged(EventArgs e)
        {
            // Base call
            base.OnCheckedChanged(e);

            // State
            _frame = 0;
            _state = Checked;

            // Are we in design time? Updates look of check-box
            // in design time.
            if (DesignMode)
            {
                // Is check-box checked?
                if (Checked)
                {
                    // Make check-box checked
                    _a = 10;
                    _frame = _checkmark.Length;
                }
                else
                {
                    // Make check-box unchecked
                    _a = 0;
                    _frame = 0;
                }
            }

            // Calls OnPaint method
            Invalidate();
        }

        /// <summary>
        /// Draws check-mark
        /// </summary>
        private void DrawCheckmarkIn(Graphics g, int yOffset = 0, int width = 2)
        {
            // Draws check-mark
            for (int i = 0; i < _checkmark.Length; i++)
            {
                if (i + 1 < _checkmark.Length)
                {
                    if (i < _frame)
                    {
                        using (Pen pen = new Pen(Parent.BackColor, width))
                            g.DrawLine(pen, new Point(_checkmark[i].X, _checkmark[i].Y + yOffset), new Point(_checkmark[i + 1].X, _checkmark[i + 1].Y + yOffset));
                    }
                }
            }
        }

        /// <summary>
        /// Draws check-mark backwards
        /// </summary>
        private void DrawCheckmarkOut(Graphics g, int yOffset = 0, int width = 2)
        {
            // Draws check-mark
            for (int i = _checkmark.Length - 1; i > 0; i--)
            {
                if (i < (_checkmark.Length - _frame))
                {
                    using (Pen pen = new Pen(Parent.BackColor, width))
                        g.DrawLine(pen, new Point(_checkmark[i].X, _checkmark[i].Y + yOffset), new Point(_checkmark[i - 1].X, _checkmark[i - 1].Y + yOffset));
                }
            }
        }
    }
}