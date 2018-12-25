using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using Minimal.Internal;
using Minimal.External;
using Minimal.Themes;
using Minimal.Components.Core;

namespace Minimal.Components
{
    /// <summary>
    /// Track-bar control
    /// </summary>
    [Designer("Minimal.Components.Designer.TrackbarDesigner")]
    public partial class MTrackbar : UserControl, IMComponent
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
        /// Final value
        /// </summary>
        private int _value;

        /// <summary>
        /// Value changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when Value is changed.")]
        public event PropertyChangedEventHandler ValueChanged = null;

        /// <summary>
        /// Value property
        /// </summary>s
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Category("Minimal")]
        [Description("Final value of the track-bar.")]
        public int Value
        {
            get { return _value; }
            set
            {
                if (value != this._value)
                {
                    // Update value
                    _value = value;

                    // Updates track-button position
                    HandleTrackButtonPosition();

                    // Trigger value-changed event
                    TriggerValueChangedEvent();

                    // Update
                    Component.Outdated = true;
                }
            }
        }

        /// <summary>
        /// Minimum value
        /// </summary>
        private double _minimum;

        /// <summary>
        /// Minimum changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the Minimum is changed.")]
        public event PropertyChangedEventHandler MinimumChanged;

        /// <summary>
        /// Minimum property
        /// </summary>
        [Category("Minimal")]
        [Description("Minimum possible value of track-bar.")]
        public double Minimum
        {
            get { return _minimum; }
            set
            {
                if (value != _minimum)
                {
                    // Update value
                    _minimum = value;

                    // Trigger event
                    MinimumChanged?.Invoke(this, new PropertyChangedEventArgs("MinimumChanged"));

                    // Refresh control
                    Refresh();

                    // Update
                    Component.Outdated = true;
                }
            }
        }

        /// <summary>
        /// Maximum value
        /// </summary>
        private double _maximum;

        /// <summary>
        /// Maximum changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the Maximum is changed.")]
        public event PropertyChangedEventHandler MaximumChanged;

        /// <summary>
        /// Maximum property
        /// </summary>
        [Category("Minimal")]
        [Description("Maximum possible value of track-bar.")]
        public double Maximum
        {
            get { return _maximum; }
            set
            {
                if (value != _maximum)
                {
                    // Update value
                    _maximum = value;

                    // Trigger maximum-changed event
                    MaximumChanged?.Invoke(this, new PropertyChangedEventArgs("MaximumChanged"));

                    // Refresh control
                    Refresh();

                    // Update
                    Component.Outdated = true;
                }
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
        [Description("Automatically hides slider when true.")]
        public bool AutoHide
        {
            get { return _autoHide; }
            set
            {
                if (value != _autoHide)
                {
                    // Update value
                    _autoHide = value;

                    // Trigger event
                    AutoHideChanged?.Invoke(this, new PropertyChangedEventArgs("AutoHideChanged"));

                    // Update
                    Component.Outdated = true;
                }

                // Redraw
                Invalidate();
            }
        }


        /// <summary>
        /// Increase value gradually
        /// </summary>
        private bool _increaseValueGradually;

        /// <summary>
        /// Increase value gradually event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the increaseValueGradually is changed.")]
        public event PropertyChangedEventHandler IncreaseValueGraduallyChanged;

        /// <summary>
        /// IncreaseValueGradually property
        /// </summary>
        [Category("Minimal")]
        [Description("Increases value gradually after click on value bar.")]
        public bool IncreaseValueGradually
        {
            get { return _increaseValueGradually; }
            set
            {
                if (value != _increaseValueGradually)
                {
                    // Update value
                    _increaseValueGradually = value;

                    // Trigger event
                    IncreaseValueGraduallyChanged?.Invoke(this, new PropertyChangedEventArgs("IncreaseValueGraduallyChanged"));

                    // Update
                    Component.Outdated = true;
                }

                // Redraw
                Invalidate();
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
        /// Track-button rectangle
        /// </summary>
        private Rectangle _trackButtonRectangle;

        /// <summary>
        /// Current position of track-button
        /// </summary>
        private Point _trackButtonCurrentPosition;

        /// <summary>
        /// Clicked position of track-button
        /// </summary>
        private Point _trackButtonClickPosition;

        /// <summary>
        /// Temporary track-button position
        /// </summary>
        private Point _tempTrackButtonPosition;

        /// <summary>
        /// Current radius of track-button
        /// </summary>
        private int _trackButtonCurrentRadius;

        /// <summary>
        /// Default track-button radius
        /// </summary>
        private int _trackButtonRadius;

        /// <summary>
        /// Radius of track-button on hover
        /// </summary>
        private int _trackButtonHoverRadius;

        /// <summary>
        /// Track-button pen width
        /// </summary>s
        private int _trackButtonPenWidth;

        /// <summary>
        /// Track-path width
        /// </summary>
        private int _trackPathWidth;

        /// <summary>
        /// Inner padding of slider
        /// </summary>
        private int _padding;

        /// <summary>
        /// True if user clicks on track-button
        /// </summary>
        private bool _trackButtonClick;

        /// <summary>
        /// True if user click on track-path
        /// </summary>
        private bool _trackPathClick;

        /// <summary>
        /// True if mouse hovers over control
        /// </summary>
        private bool _mouseControlHover;

        /// <summary>
        /// Start point of track bar
        /// </summary>
        private Point _trackStart;

        /// <summary>
        /// End point of track bar
        /// </summary>
        private Point _trackEnd;

        /// <summary>
        /// Mouse position
        /// </summary>
        private Point _mouse;

        /// <summary>
        /// Track-path length
        /// </summary>
        private double _trackPathLength;

        /// <summary>
        /// Increment per pixel
        /// </summary>
        private double _increment;

        /// <summary>
        /// Distance between track-path zero point and track-button
        /// </summary>
        private double _trackButtonDistance;

        /// <summary>
        /// Animation of slider move progress in milliseconds
        /// </summary>
        private double _tTrackButton;

        /// <summary>
        /// Constructor
        /// </summary>
        public MTrackbar()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialize control
            InitializeComponent();

            // Default variables
            _value = 0;
            _minimum = 0;
            _maximum = 100;
            _autoHide = false;
            _tempTrackButtonPosition = new Point(0, 0);
            _trackButtonCurrentRadius = 9;
            _trackButtonRadius = 9;
            _trackButtonHoverRadius = 8;
            _trackButtonPenWidth = 4;
            _trackPathLength = Geometry.GetDistanceBetweenPoints(_trackStart, _trackEnd);
            _trackPathWidth = 5;
            _padding = _trackButtonRadius + _trackButtonHoverRadius + 1;
            _trackButtonClick = false;
            _trackButtonCurrentPosition.Y = Height / 2;
            _trackPathClick = false;
            _mouseControlHover = false;
            _tTrackButton = 0;
            DoubleBuffered = true;
            Size = new Size(200, 35);

            // Auto-hide
            if (_autoHide)
                _trackButtonCurrentRadius = 0;
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

            // Call update manually
            UpdateComponent(null, EventArgs.Empty);
        }

        /// <summary>
        /// Update
        /// </summary>
        private void UpdateComponent(object sender, EventArgs e)
        {
            // Animation variables
            _tTrackButton += 0.025;

            // Track button
            _trackButtonRectangle = new Rectangle(_trackButtonCurrentPosition.X - _trackButtonCurrentRadius, _trackButtonCurrentPosition.Y - _trackButtonCurrentRadius, _trackButtonCurrentRadius * 2, _trackButtonCurrentRadius * 2);
            _trackButtonClickPosition.Y = Height / 2;
            _trackButtonCurrentPosition.Y = Height / 2;

            // Update important variables for animation
            if (_trackPathClick)
            {
                // Track-button click position
                _trackButtonClickPosition.X = _mouse.X;

                // Temporary track-button position
                _tempTrackButtonPosition = _trackButtonCurrentPosition;

                // Animation progress of track-button
                _tTrackButton = 0;

                // Value changed event
                TriggerValueChangedEvent();
            }

            // Did user clicked on track-button?
            if (_trackButtonClick)
            {
                // Updates both click and current positions of track-bar
                _trackButtonClickPosition.X = _mouse.X;
                _trackButtonCurrentPosition.X = _mouse.X;

                // Value changed event
                TriggerValueChangedEvent();
            }

            // Limit real minimal horizontal position of slider from left, so half
            // of track button don't overlap track bar
            if (_trackButtonClickPosition.X < _trackStart.X)
            {
                // Click position
                _trackButtonClickPosition.X = _trackStart.X;

                // Value changed event
                TriggerValueChangedEvent();
            }

            // Limit real minimal horizontal position of slider from left, so half
            // of track button don't overlap track bar
            if (_trackButtonCurrentPosition.X < _trackStart.X)
            {
                // Current position
                _trackButtonCurrentPosition.X = _trackStart.X;

                // Value changed event
                TriggerValueChangedEvent();
            }

            // Limit real minimal horizontal position of slider from right, so half
            // of track button don't overlap track bar
            if (_trackButtonClickPosition.X > _trackEnd.X)
            {
                // Click position
                _trackButtonClickPosition.X = _trackEnd.X;

                // Value changed event
                TriggerValueChangedEvent();
            }

            // Limit real minimal horizontal position of slider from right, so half
            // of track button don't overlap track bar
            if (_trackButtonCurrentPosition.X > _trackEnd.X)
            {
                // Click position
                _trackButtonCurrentPosition.X = _trackEnd.X;

                // Value changed event
                TriggerValueChangedEvent();
            }

            // Animation of track button when user clicks on track bar
            if (_trackButtonCurrentPosition.X < _trackButtonClickPosition.X)
            {
                // Current position
                _trackButtonCurrentPosition.X = _tempTrackButtonPosition.X + Animation.CosinusMotion(_tTrackButton, (int)Geometry.GetDistanceBetweenPoints(_tempTrackButtonPosition, _trackButtonClickPosition));

                // Value changed event
                TriggerValueChangedEvent();
            }

            // Animation of track button when user clicks on track bar
            if (_trackButtonCurrentPosition.X > _trackButtonClickPosition.X)
            {
                // Current position
                _trackButtonCurrentPosition.X = _tempTrackButtonPosition.X - Animation.CosinusMotion(_tTrackButton, (int)Geometry.GetDistanceBetweenPoints(_trackButtonClickPosition, _tempTrackButtonPosition));

                // Value changed event
                TriggerValueChangedEvent();
            }

            // Handles auto-hide
            if (_autoHide)
            {
                // Reduce track-button size if mouse is not hovering over it
                if (!_mouseControlHover)
                {
                    // Is current radius bigger than zero?
                    if (_trackButtonCurrentRadius > 0)
                    {
                        // Reduce radius
                        _trackButtonCurrentRadius--;
                    }
                }
                else
                {
                    // Increase radius
                    if (_trackButtonCurrentRadius < _trackButtonRadius)
                        _trackButtonCurrentRadius++;
                }
            }

            // If auto-hide is not active - fix auto-hide toggle
            if (!_autoHide)
            {
                // Is current radius smaller than actual track-button radius?
                if (_trackButtonCurrentRadius < _trackButtonRadius)
                {
                    // Increment current radius
                    _trackButtonCurrentRadius++;
                }
            }

            // Limit animation variables
            if (_tTrackButton > 1)
                _tTrackButton = 1;

            // Distance between track-bar start point and track button
            _trackButtonDistance = (int) Geometry.GetDistanceBetweenPoints(_trackStart, (_increaseValueGradually) ? _trackButtonCurrentPosition : _trackButtonClickPosition);

            // Length of track bar
            _trackPathLength = (int) Geometry.GetDistanceBetweenPoints(_trackStart, _trackEnd);

            // Increment per pixel
            _increment = (_maximum - _minimum) / _trackPathLength;

            // Value of slider
            _value = Convert.ToInt32(_minimum + (int) Math.Round((_trackButtonDistance) * _increment, MidpointRounding.AwayFromZero));

            // Disable updates if all animations are finished
            if ((_tTrackButton == 0 || _tTrackButton == 1) && (_trackButtonCurrentRadius == _trackButtonRadius || _trackButtonCurrentRadius == 0))
                Component.Outdated = false;

            // Redraw
            Invalidate();
        }

        /// <summary>
        /// Draw method
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base painting
            base.OnPaint(e);

            // Get graphics context
            Graphics g = e.Graphics;

            // Clear control
            g.Clear(Parent.BackColor);

            // Set anti-aliasing
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw track-path
            Color trackColor = Component.SourceTheme.COMPONENT_FILL.Normal.ToColor();

            // Creates track-path pen
            using (Pen trackPen = new Pen(trackColor, _trackPathWidth))
            {
                // Modify caps of line
                trackPen.StartCap = LineCap.Round;
                trackPen.EndCap = LineCap.Round;

                // Draws line
                g.DrawLine(trackPen, _trackStart, _trackEnd);
            }

            // Track-button hover effect
            if (_trackButtonRectangle.Contains(_mouse) && !_trackButtonClick)
            {
                // Creates track-button ellipse
                GraphicsPath trackButtonEllipseCenterActive = Draw.GetEllipsePath(_trackButtonCurrentPosition, _trackButtonCurrentRadius + _trackButtonHoverRadius);

                // Draw track-button active
                using (SolidBrush trackButtonBrushActive = new SolidBrush(Color.FromArgb(25, Component.Accent)))
                    g.FillPath(trackButtonBrushActive, trackButtonEllipseCenterActive);
            }

            // Get track-button ellipse
            GraphicsPath trackButtonEllipse = Draw.GetEllipsePath(_trackButtonCurrentPosition, _trackButtonCurrentRadius);

            // Is track-button ellipse visible?
            if (_trackButtonCurrentRadius > 0)
            {
                // Draw track-button
                using (Pen trackButtonPen = new Pen(Component.Accent, _trackButtonPenWidth))
                    g.DrawPath(trackButtonPen, trackButtonEllipse);
            }

            // Repaint track-path with tint color
            using (Pen trackTintPen = new Pen(Component.Accent, _trackPathWidth))
            {
                // Modify caps
                trackTintPen.StartCap = LineCap.Round;
                trackTintPen.EndCap = LineCap.Round;

                // Draws active path
                g.DrawLine(trackTintPen, _trackStart, new Point(_trackButtonCurrentPosition.X, _trackButtonCurrentPosition.Y));
            }

            // Erase middle of track-button
            // Get erase color
            Color trackButtonColor = Parent.BackColor;

            // Get graphics-path middle of track-button
            GraphicsPath trackButtonEllipseCenter = Draw.GetEllipsePath(_trackButtonCurrentPosition, _trackButtonCurrentRadius - 1);

            // Is track-button visible?
            if (_trackButtonCurrentRadius > 0)
            {
                // Draw hollow ellipse on top of track button to erase its middle
                using (SolidBrush trackButtonBrush = new SolidBrush(trackButtonColor))
                    g.FillPath(trackButtonBrush, trackButtonEllipseCenter);
            }

            // Track-button click effect
            // IS track-button clicked?
            if (_trackButtonClick)
            {
                // Get graphics-path
                GraphicsPath trackButtonEllipseCenterActive = Draw.GetEllipsePath(_trackButtonCurrentPosition, _trackButtonCurrentRadius - _trackButtonPenWidth + 4);

                // Draws click-effect
                using (SolidBrush trackButtonBrushActive = new SolidBrush(Component.Accent))
                    g.FillPath(trackButtonBrushActive, trackButtonEllipseCenterActive);
            }

            // Ends anti-aliasing
            g.SmoothingMode = SmoothingMode.Default;
        }

        /// <summary>
        /// Mouse click
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Base call
            base.OnMouseDown(e);

            // If mouse hovers over track-button
            if (_trackButtonRectangle.Contains(e.X, e.Y))
                _trackButtonClick = true;

            // If mouse hovers over track-path
            if (new Rectangle(0, Height / 2 - 6, Width - _padding, _trackPathWidth + 8).Contains(e.X, e.Y))
                _trackPathClick = true;

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Mouse release
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Base call
            base.OnMouseUp(e);

            // Reset track-button click
            _trackButtonClick = false;

            // Reset track-path click
            _trackPathClick = false;

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Mouse position update
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Base call
            base.OnMouseMove(e);

            // Update mouse position
            _mouse = new Point(e.X, e.Y);

            // Is track-button clicked?
            if (_trackButtonClick)
            {
                // Update track-button click position
                _trackButtonClickPosition = _mouse;

                // Trigger value changed method
                TriggerValueChangedEvent();
            }

            // Check if mouse is in control area
            if (ClientRectangle.Contains(_mouse))
            {
                // Update hover
                _mouseControlHover = true;
            }

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Mouse leave
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            // base call
            base.OnMouseLeave(e);

            // Reset hover
            _mouseControlHover = false;

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Resize
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            // Base call
            base.OnResize(e);

            // Start and end points
            _trackStart = new Point(_padding, Height / 2);
            _trackEnd = new Point(Width - _padding, Height / 2);

            // Limit width
            if (Width < 100)
                Width = 100;

            // Limit height
            Height = 35;

            // Recalculate track-button position
            HandleTrackButtonPosition();

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Handle track-button position
        /// </summary>
        private void HandleTrackButtonPosition()
        {
            // Update important variables
            _trackPathLength = Geometry.GetDistanceBetweenPoints(_trackStart, _trackEnd);
            _increment = _trackPathLength / (_maximum - _minimum);
            _trackButtonClickPosition.X = Convert.ToInt32(_value * _increment) + _padding - (int)(_minimum * _increment);
            _trackButtonCurrentPosition.X = Convert.ToInt32(_value * _increment) + _padding - (int)(_minimum * _increment);

            // Redraw
            Invalidate();

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Trigger value changed event
        /// </summary>
        private void TriggerValueChangedEvent()
        {
            // Trigger value-changed event
            ValueChanged?.Invoke(this, new PropertyChangedEventArgs("ValueChanged"));

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Plays track-bar’s Value animation.
        /// </summary>
        public void Animate()
        {
            // Update variables
            _tTrackButton = 0;
            _trackButtonCurrentPosition = new Point(_trackStart.X, Height / 2);
            _tempTrackButtonPosition = new Point(_trackButtonCurrentPosition.X, _trackButtonCurrentPosition.Y);

            // Limit current position <
            if (_trackButtonCurrentPosition.X < _trackButtonClickPosition.X)
                _trackButtonCurrentPosition.X = _tempTrackButtonPosition.X + Animation.CosinusMotion(_tTrackButton, (int)Geometry.GetDistanceBetweenPoints(_tempTrackButtonPosition, _trackButtonClickPosition));

            // Limit current position >
            if (_trackButtonCurrentPosition.X > _trackButtonClickPosition.X)
                _trackButtonCurrentPosition.X = _tempTrackButtonPosition.X - Animation.CosinusMotion(_tTrackButton, (int)Geometry.GetDistanceBetweenPoints(_trackButtonClickPosition, _tempTrackButtonPosition));

            // Update
            Component.Outdated = true;
        }
    }
}
