using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Minimal.Themes;
using Minimal.External;
using Minimal.Internal;
using Minimal.Components.Core;

namespace Minimal.Components
{
    /// <summary>
    /// Simple color box
    /// </summary>
    [Designer("Minimal.Components.Designer.ColorBoxDesigner")]
    [DefaultEvent("SelectedChanged")]
    public partial class MColorBox : MBufferedPanel, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        public MComponent Component { get; set; }

        /// <summary>
        /// Color property
        /// </summary>
        [Category("Minimal")]
        [Description("Color which should color-box hold.")]
        public Color Color { get; set; }

        /// <summary>
        /// Selected changed event
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when Selected property is changed.")]
        public event PropertyChangedEventHandler SelectedChanged;

        /// <summary>
        /// Selected local variable
        /// </summary>
        private bool _selected = false;

        /// <summary>
        /// True if color-box is selected
        /// </summary>
        [Category("Minimal")]
        [Description("True if color-box is selected.")]
        public bool Selected
        {
            get
            {
                return _selected;
            }

            set
            {
                if (_selected != value)
                {
                    // Update value
                    _selected = value;

                    // Trigger SelectedItem changed event
                    SelectedChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItemChanged"));

                    // State
                    _frame = 0;

                    // Are we in design time? Updates look of check-box
                    // in design time.
                    if (DesignMode)
                    {
                        // Is check-box checked?
                        if (Selected)
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
                    Component.Outdated = true;
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
        /// True if color-box should be have like check-box
        /// </summary>
        [Category("Minimal")]
        [Description("True, if there can be multiple color-boxes checked at the same time on the same parent control.")]
        public bool MultipleTicks { get; set; }

        /// <summary>
        /// Type of color-box
        /// </summary>
        [Category("Minimal")]
        [Description("Type of the color-box.")]
        public ColorBoxType Type { get; set; } = ColorBoxType.Rectangle;

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
        /// Constructor
        /// </summary>
        public MColorBox()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialize control
            InitializeComponent();

            // Default properties and variables
            ForeColor = Color.White;
            Color = Hex.blue.ToColor();

            // Default size
            Size = new Size(25, 25);
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
            Component.ComponentUpdate += OnComponentUpdate;
        }

        /// <summary>
        /// Update method
        /// </summary>
        private void OnComponentUpdate(object sender, EventArgs e)
        {
            // Animation variables
            if (Selected)
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

            // Turn off updates if necessary
            if ((_a == 0 || _a == 10) && (_frame == _checkmark.Length || _frame == 0))
                Component.Outdated = false;

            // Redraw control
            Invalidate();
        }

        /// <summary>
        /// Draws color-box
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base painting
            base.OnPaint(e);

            // Get graphics context
            Graphics g = e.Graphics;

            // Draw colored highlight border
            using (Pen pen = new Pen(ColorExtensions.AddRGB(Component.SourceTheme.DARK_BASED ? 70 : -70, Color)))
            {
                // Rectangular color-box
                if (Type == ColorBoxType.Circle)
                {
                    // Turn on anti-aliasing
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    // Clear control
                    using (SolidBrush brush = new SolidBrush(Color))
                        g.FillEllipse(brush, new Rectangle(new Point(0, 0), new Size(ClientRectangle.Width - 1, ClientRectangle.Height - 1)));

                    // Draw border
                    g.DrawEllipse(pen, new Rectangle(new Point(0, 0), new Size(ClientRectangle.Width - 1, ClientRectangle.Height - 1)));

                    // Turn off anti-aliasing
                    g.SmoothingMode = SmoothingMode.Default;
                }

                // Circular color-box
                if (Type == ColorBoxType.Rectangle)
                {
                    // Clear control
                    g.Clear(Color);

                    // Draw border
                    g.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(ClientRectangle.Width - 1, ClientRectangle.Height - 1)));
                }
            }

            // Is border enabled?
            if (Selected)
            {
                // Calculate foreground color
                Color foreColor = (ColorExtensions.PerceivedBrightness(Color) > 130 ? Color.Black : Color.White);
            }

            // Check-box's height
            int height = 19;

            // Middle of check-box
            int b = Height / 2 - height / 2;

            // Check-mark points
            _checkmark = new Point[]
            {
                new Point(7, b + 8),
                new Point(8, b + 9),
                new Point(9, b + 10),
                new Point(10, b + 11),
                new Point(11, b + 12),
                new Point(12, b + 11),
                new Point(13, b + 10),
                new Point(14, b + 9),
                new Point(15, b + 8),
                new Point(16, b + 7),
                new Point(17, b + 6),
                new Point(18, b + 5),
            };

            // Draws check-mark in animation
            if (_a == 10 && Selected == true)
            {
                // Draws two check-marks with 1 width of pen. Setting pen's
                // width to 2 pixels causes one missing pixel at bottom of the
                // check-mark.
                DrawCheckmarkIn(g);
                DrawCheckmarkIn(g, 1, 1);
            }

            // Draws check-mark out animation
            if (_a == 10 && Selected == false)
            {
                // Draws two check-marks with 1 width of pen. Setting pen's
                // width to 2 pixels causes one missing pixel at bottom of the
                // check-mark.
                DrawCheckmarkOut(g);
                DrawCheckmarkOut(g, 1, 1);
            }
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
                        using (Pen pen = new Pen((ColorExtensions.PerceivedBrightness(Color) > 130 ? Color.Black : Color.White), width))
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
                    using (Pen pen = new Pen((ColorExtensions.PerceivedBrightness(Color) > 130 ? Color.Black : Color.White), width))
                        g.DrawLine(pen, new Point(_checkmark[i].X, _checkmark[i].Y + yOffset), new Point(_checkmark[i - 1].X, _checkmark[i - 1].Y + yOffset));
                }
            }
        }

        /// <summary>
        /// On mouse up event
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Base call
            base.OnClick(e);

            // User pressed left button
            if (e.Button == MouseButtons.Left)
            {
                // Update Selected property
                Selected = !Selected;

                // Trigger SelectedItem changed event
                SelectedChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItemChanged"));

                // Uncheck (unselect) all other color-boxes if check-box behavior is on
                if (!MultipleTicks)
                {
                    // Iterate over all parent controls
                    foreach (Control control in Parent.Controls)
                    {
                        // Is control MColorBox?
                        if (control is MColorBox)
                        {
                            // If control is this control, return
                            if (control == this)
                                continue;

                            // Cast control to MColorBox
                            MColorBox colorBox = control as MColorBox;

                            // Unselect color-box
                            colorBox.Selected = false;
                        }
                    }
                }

                // State
                _frame = 0;

                // Are we in design time? Updates look of check-box
                // in design time.
                if (DesignMode)
                {
                    // Is check-box checked?
                    if (Selected)
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

            // Update whole control
            Component.Outdated = true;
        }
    }
}
