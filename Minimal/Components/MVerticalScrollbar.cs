using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Minimal.Themes;
using Minimal.Components.Core;

namespace Minimal.Components
{
    /// <summary>
    /// Vertical scroll-bar
    /// </summary>
    public partial class MVerticalScrollbar : UserControl, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        public MComponent Component { get; set; }

        /// <summary>
        /// Scroll event handler
        /// </summary>
        [Category("Minimal")]
        public new event EventHandler Scroll = null;

        /// <summary>
        /// Default value
        /// </summary>
        private int _value = 0;

        /// <summary>
        /// Value changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when Value is changed.")]
        public event PropertyChangedEventHandler ValueChanged;

        /// <summary>
        /// Value property
        /// </summary>
        [Category("Minimal")]
        [Description("Value of scrollbar.")]
        public int Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    // Scroll-bar needs to be updated!
                    Component.Outdated = true;

                    // Set new value
                    SetValue(value);

                    // Trigger Value changed event
                    ValueChanged?.Invoke(this, new PropertyChangedEventArgs("ValueChanged"));

                    // Trigger scroll effect
                    Scroll?.Invoke(this, new PropertyChangedEventArgs("ScrollChanged"));
                }

                // Make scrollbar visible
                _visible = true;

                // Calls paint method
                Invalidate();
            }
        }

        /// <summary>
        /// Default minimum
        /// </summary>
        private int _minimum = 0;

        /// <summary>
        /// Minimum changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when Minimum is changed.")]
        public event PropertyChangedEventHandler MinimumChanged;

        /// <summary>
        /// Minimum property
        /// </summary>
        [Category("Minimal")]
        [Description("Minimal Value of scrollbar.")]
        public int Minimum
        {
            get { return _minimum; }
            set
            {
                if (value != _minimum)
                {
                    _minimum = value;
                    MinimumChanged?.Invoke(this, new PropertyChangedEventArgs("MinimumChanged"));
                }

                // Calls paint method
                Invalidate();
            }
        }

        /// <summary>
        /// Default maximum
        /// </summary>
        private int _maximum = 100;

        /// <summary>
        /// Maximum changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when Maximum is changed.")]
        public event PropertyChangedEventHandler MaximumChanged;

        /// <summary>
        /// Maximum property
        /// </summary>
        [Category("Minimal")]
        [Description("Maximal Value of scrollbar.")]
        public int Maximum
        {
            get { return _maximum; }
            set
            {
                if (value != _maximum)
                {
                    _maximum = value;
                    MaximumChanged?.Invoke(this, new PropertyChangedEventArgs("MaximumChanged"));
                }

                // Calls paint method
                Invalidate();
            }
        }

        /// <summary>
        /// Length of cursor
        /// </summary>
        private int _cursorLength = 50;

        /// <summary>
        /// CursorLength changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when cursor length is changed.")]
        public event PropertyChangedEventHandler CursorLengthChanged;

        /// <summary>
        /// CursorLength property
        /// </summary>
        [Category("Minimal")]
        [Description("Cursor length.")]
        public int CursorLength
        {
            get { return _cursorLength; }
            set
            {
                if (value != _cursorLength)
                {
                    _cursorLength = value;
                    CursorLengthChanged?.Invoke(this, new PropertyChangedEventArgs("CursorLengthCahnged"));
                }

                // Calls paint method
                Invalidate();
            }
        }

        /// <summary>
        /// Auto-hide
        /// </summary>
        private bool _autoHide;

        /// <summary>
        /// AutoHide event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the AutoHide is changed.")]
        public event PropertyChangedEventHandler AutoHideChanged;

        /// <summary>
        /// AutoHide property
        /// </summary>
        [Category("Minimal")]
        [Description("Automatically hides scrollbar when true.")]
        public bool AutoHide
        {
            get { return _autoHide; }
            set
            {
                if (value != _autoHide)
                {
                    _autoHide = value;
                    AutoHideChanged?.Invoke(this, new PropertyChangedEventArgs("AutoHideChanged"));
                }

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// Custom theme local variable
        /// </summary>
        private Theme _customTheme;

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
        /// Minified local variable
        /// </summary>
        private bool _minified;

        /// <summary>
        /// Minified changed event
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when Minified is changed.")]
        public event PropertyChangedEventHandler MinifiedChanged;

        /// <summary>
        /// Minified property
        /// </summary>
        [Category("Minimal")]
        [Description("True if scrollbar should be minified.")]
        public bool Minified
        {
            get { return _minified; }
            set
            {
                // Change used theme
                _minified = value;

                // Update width
                if (Minified)
                    Width = 5;
                else
                    Width = 20;

                // Fire event
                MinifiedChanged?.Invoke(this, new PropertyChangedEventArgs("MinifiedChanged"));

                // Redraw control
                Invalidate(true);
            }
        }

        /// <summary>
        /// Mouse click position
        /// </summary>
        private Point _mouseClickPosition;

        /// <summary>
        /// Mouse increment
        /// </summary>
        private int _mouseIncrement;

        /// <summary>
        /// Track-bar
        /// </summary>
        private Rectangle _cursorRectangle;

        /// <summary>
        /// Position of cursor
        /// </summary>
        private Point _cursorPosition;

        /// <summary>
        /// Previous cursor position
        /// </summary>
        private Point _cursorPositionPrevious;

        /// <summary>
        /// True if mouse cursor hovers over scrollbar's cursor
        /// </summary>
        private bool _cursorHover;

        /// <summary>
        /// True if mouse hovers over control
        /// </summary>
        private bool _hover;

        /// <summary>
        /// Timer controls how long can scrollbar stay visible
        /// </summary>
        private int _timer;

        /// <summary>
        /// true if user clicks on scrollbar's cursor
        /// </summary>
        private bool _cursorClick;

        /// <summary>
        /// Alpha of scrollbar
        /// </summary>
        private int _alpha;

        /// <summary>
        /// True if scrollbar is hidden
        /// </summary>
        private bool _visible;

        /// <summary>
        /// Constructor
        /// </summary>
        public MVerticalScrollbar()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialize
            InitializeComponent();

            // Default variables
            _cursorRectangle = new Rectangle();
            _cursorPosition = new Point(0, 0);
            _cursorHover = false;
            _cursorClick = false;
            _visible = false;
            _autoHide = true;
            _alpha = (LicenseManager.UsageMode == LicenseUsageMode.Designtime) ? 255 : 0;
            _timer = 0;
            _customTheme = null;
            Width = 20;
            DoubleBuffered = true;
            AutoHide = false;
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
            // If control is fully hidden
            if (Width == 0)
            {
                // If mouse is in are of the control
                if (new Rectangle(new Point(), new Size(20, Height)).Contains(PointToClient(Cursor.Position)))
                {
                    _visible = true;
                }
            }

            // Update cursor
            _cursorRectangle = new Rectangle(_cursorPosition, new Size(Width, _cursorLength));

            // Update mouse increment
            _mouseIncrement = PointToClient(Cursor.Position).Y - _mouseClickPosition.Y;

            // Update cursor position
            if (_cursorClick)
            {
                // Update cursor position
                _cursorPosition = new Point(0, _cursorPositionPrevious.Y + _mouseIncrement);

                // Trigger scroll event
                Scroll?.Invoke(this, new EventArgs());

                // Trigger value changed event
                ValueChanged?.Invoke(this, new PropertyChangedEventArgs("ValueChanged"));

                // Make scroll-bar visible
                _visible = true;
            }

            // Limit cursor position
            // Minimal
            if (_cursorPosition.Y < 0)
                _cursorPosition.Y = 0;

            // Maximal
            if (_cursorPosition.Y + _cursorLength > Height)
                _cursorPosition.Y = Height - _cursorLength;

            // Update value
            _value = GetValue();

            // Call mouse leave method
            try { if (!ClientRectangle.Contains(PointToClient(MousePosition))) { base.OnMouseLeave(EventArgs.Empty); } } catch { }

            // Timer
            if (_visible && !_hover)
            {
                // Increment timer
                _timer++;

                // Once timer tick 200 ticks
                if ((_timer % 200) == 0)
                {
                    // Restart timer
                    _timer = 0;

                    // Make scroll-bar invisible
                    _visible = false;
                }
            }

            // Should scroll-bar hide itself after some time?
            if (AutoHide)
            {
                // Scroll-bar should hide
                // Is scrollbar visible?
                if (_visible)
                {
                    // Reveal effect
                    // Increment alpha
                    if (_alpha < 255)
                        _alpha += 15;

                    // Increase width
                    if (Width < (_minified ? 5 : 20))
                        Width++;
                }
                else
                {
                    // Hide effect
                    // Decrease alpha
                    if (_alpha > 0)
                        _alpha -= 15;

                    // Decrease width
                    if (Width > 0)
                        Width--;
                }
            }
            else
            {
                // Scrollbar is never hidden
                // Set full opacity (alpha)
                _alpha = 255;
            }

            // Handle scroll-bar self update routine
            if (AutoHide)
            {
                // If AutoHide is on, scroll-bar needs to be constantly updated
                Component.Outdated = true;
            }
            else
            {
                // If all animations are done
                if ((Width == 0 || Width == 20) && (_alpha == 0 || _alpha == 255))
                {
                    // No need for updates
                    Component.Outdated = false;
                }
            }

            // Redraw scrollbar
            Invalidate();
        }

        /// <summary>
        /// Draw method
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base call
            base.OnPaint(e);

            // Get graphics context
            Graphics g = e.Graphics;

            // Clear control
            g.Clear(Parent.BackColor);

            // Color of path
            Color color = Color.FromArgb(_alpha, Component.SourceTheme.COMPONENT_FILL.Normal.ToColor());

            // Draw path
            using (SolidBrush brush = new SolidBrush(color))
                g.FillRectangle(brush, ClientRectangle);

            // Cursor colors
            Color normalColor = Color.FromArgb(_alpha, Component.SourceTheme.COMPONENT_HIGHLIGHT.Normal.ToColor());
            Color hoverColor = Color.FromArgb(_alpha, Component.SourceTheme.COMPONENT_HIGHLIGHT.Hover.ToColor());

            // If there is enough space to scroll
            if (_cursorRectangle.Height != Height)
            {
                // Define brushes
                using (SolidBrush hover = new SolidBrush(hoverColor))
                {
                    using (SolidBrush normal = new SolidBrush(normalColor))
                    {
                        // Draw scroll thumb (cursor)
                        g.FillRectangle((_cursorClick) ? hover : normal, _cursorRectangle);
                    }
                }
            }
        }

        /// <summary>
        /// Resize event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            // Base call
            base.OnResize(e);

            // Update cursor
            SetValue(_value);

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Mouse move
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Base call
            base.OnMouseMove(e);

            // Scroll-bar needs to be updated!
            Component.Outdated = true;

            // Check if mouse hover over scrollbar's cursor
            if (_cursorRectangle.Contains(e.Location))
                _cursorHover = true;
            else
                _cursorHover = false;

            // Update mouse coordinates
            Point mouse = e.Location;

            // Update mouse increment
            _mouseIncrement = mouse.Y - _mouseClickPosition.Y;
        }

        /// <summary>
        /// Return Value based on cursor position
        /// </summary>
        private int GetValue()
        {
            // Get path
            decimal path = Height - _cursorLength;

            // Catch divide by zero exception
            if (path > 0 && (_maximum - _minimum > 0))
            {
                decimal increment = (_maximum - _minimum) / path;
                return _minimum + Convert.ToInt32(increment * _cursorPosition.Y);
            }

            // Calculation not successful
            return _value;
        }

        /// <summary>
        /// Set Value and updates cursor position
        /// </summary>
        private void SetValue(int value)
        {
            // Update value
            _value = value;

            // Update cursor
            decimal path = Height - _cursorLength;

            // Catch divide by zero exception
            if (path > 0 && (_maximum - _minimum > 0))
            {
                // Calculate increment
                decimal increment = path / (_maximum - _minimum);

                // Update cursor (scroll-thumb) position
                _cursorPosition.Y = Convert.ToInt32(_value * increment) + (int)(_minimum * increment);
            }

            // Reset timer
            _timer = 0;

            // Redraw
            Invalidate();
        }

        /// <summary>
        /// Mouse click
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Base call
            base.OnMouseClick(e);

            // Get focus
            Focus();

            // Scroll-bar needs to be updated!
            Component.Outdated = true;

            // Check if user clicks on scrollbar's cursor
            if (_cursorHover)
                _cursorClick = true;

            // Mouse click position
            _mouseClickPosition = e.Location;

            // Store current cursor position to previous cursor position
            _cursorPositionPrevious = _cursorPosition;
        }

        /// <summary>
        /// Mouse release
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Base call
            base.OnMouseUp(e);

            // Scroll-bar needs to be updated!
            Component.Outdated = true;

            // Reset cursor click
            _cursorClick = false;
        }

        /// <summary>
        /// Mouse enter
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            // Base call
            base.OnMouseEnter(e);

            // Scroll-bar needs to be updated!
            Component.Outdated = true;

            // Update hover
            _hover = true;

            // Make scrollbar visible
            _visible = true;
        }

        /// <summary>
        /// Mouse leave
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            // Base call
            base.OnMouseLeave(e);

            // Scroll-bar needs to be updated!
            Component.Outdated = true;

            // Update cursor hover
            _cursorHover = false;

            // Update hover
            _hover = false;
        }

        /// <summary>
        /// Makes scrollbar visible
        /// </summary>
        public void Wake()
        {
            _visible = true;
        }

        /// <summary>
        /// Hides scroll-bar
        /// </summary>
        public void Sleep()
        {
            _visible = false;
        }

        /// <summary>
        /// Forces to update scroll-bar
        /// </summary>
        internal void ForceUpdate()
        {
            Component.Outdated = true;
        }
    }
}
