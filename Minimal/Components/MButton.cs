using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.ComponentModel.Design;
using Minimal.Themes;
using Minimal.External;
using Minimal.Internal;
using Minimal.Components.Core;

namespace Minimal.Components
{
    /// <summary>
    /// Button component
    /// </summary>
    [Designer("Minimal.Components.Designer.ButtonDesigner")]
    public partial class MButton : Button, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        [Category("Minimal")]
        [Description("Handles life-cycle of the M-Component.")]
        public MComponent Component { get; set; }

        /// <summary>
        /// Default click effect
        /// </summary>
        private ClickEffect _clickEffect = ClickEffect.Ripple;

        /// <summary>
        /// Click effect changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the ClickEffect property is changed.")]
        public event PropertyChangedEventHandler ClickEffectChanged;

        /// <summary>
        /// ClickEffect property
        /// </summary>
        [Category("Minimal")]
        [Description("Component's click effect.")]
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
        /// Default button type
        /// </summary>
        private ButtonType _buttonType = ButtonType.Raised;

        /// <summary>
        /// Button type changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the Type property is changed.")]
        public event PropertyChangedEventHandler TypeChanged;

        /// <summary>
        /// Type property
        /// </summary>
        [Category("Minimal")]
        [Description("Type of the button.")]
        public ButtonType Type
        {
            get { return _buttonType; }
            set
            {
                if (value != _buttonType)
                {
                    // Update actual button-type value
                    _buttonType = value;

                    // Invoke property changed event handler
                    TypeChanged?.Invoke(this, new PropertyChangedEventArgs("TypeChanged"));

                    // Redraw component
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Default custom color
        /// </summary>
        private bool _customColor = false;

        /// <summary>
        /// Custom color property changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the CustomColor property is changed.")]
        public event PropertyChangedEventHandler CustomColorChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the control should be fully painted with his accent color.
        /// </summary>
        [Category("Minimal")]
        [Description("Fills the button with his Accent color if true.")]
        public bool CustomColor
        {
            get { return _customColor; }
            set
            {
                if (value != _customColor)
                {
                    // Update actual custom-color value
                    _customColor = value;

                    // Invoke property changed event handler 
                    CustomColorChanged?.Invoke(this, new PropertyChangedEventArgs("CustomColorChanged"));

                    // Redraw component
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Default text-in-capital
        /// </summary>
        private bool _capitalText = true;

        /// <summary>
        /// Capital text property changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the TextInCapital property is changed.")]
        public event PropertyChangedEventHandler CapitalTextChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the control's text should be in capital
        /// </summary>
        [Category("Minimal")]
        [Description("True if the button's text should be in the capital.")]
        public bool CapitalText
        {
            get { return _capitalText; }
            set
            {
                if (value != _capitalText)
                {
                    // Update actual capital-text value
                    _capitalText = value;

                    // Invoke property changed event handler 
                    CapitalTextChanged?.Invoke(this, new PropertyChangedEventArgs("CapitalTextChanged"));

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
        /// True if the mouse is inside of the component
        /// </summary>
        private bool _hover;

        /// <summary>
        /// Alpha value of the hover effect which is later added to control fill color
        /// </summary>
        private byte _hoverAlpha;

        /// <summary>
        /// Mouse position when user clicks
        /// </summary>
        private Point _mouseClickPosition;

        /// <summary>
        /// Mouse position relative to the control's top left corner
        /// </summary>
        private Point _mouse;

        /// <summary>
        /// Length of the button diagonal
        /// </summary>
        private int _diagonal;

        /// <summary>
        /// Radius of the click effect
        /// </summary>
        private double _radius;

        /// <summary>
        /// Radius increment of the click effect
        /// </summary>
        private double _increment;

        /// <summary>
        /// Alpha value of the click effect
        /// </summary>
        private byte _clickAlpha;

        /// <summary>
        /// Rotation of the click effect (in degrees)
        /// </summary>
        private int _rotation;

        /// <summary>
        /// Accent alpha
        /// </summary>
        private byte _accentAlpha;

        /// <summary>
        /// Reveal effect distance
        /// </summary>
        private int _revealDistance;

        /// <summary>
        /// Constructor
        /// </summary>
        public MButton()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialization
            InitializeComponent();

            // Variables
            _hover = false;
            _hoverAlpha = 0;
            _increment = 1;
            _accentAlpha = 0;
            _revealDistance = 105;

            // Default font
            Font = new Font("Segoe UI", 8);

            // Default size
            Size = new Size(130, 36);

            // Turn on double-buffer
            DoubleBuffered = true;

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
            Component.ComponentUpdate += OnComponentUpdate;
        }

        /// <summary>
        /// Update method
        /// </summary>
        private void OnComponentUpdate(object sender, EventArgs e)
        {
            // Hover effect
            if (_hover)
            {
                // Increase hover alpha
                if (_hoverAlpha < 255)
                    _hoverAlpha += 15;
            }
            else
            {
                // Decrease hover alpha
                if (_hoverAlpha > 0)
                    _hoverAlpha -= 15;
            }

            // ClickEffect
            // If radius is smaller than half of diagonal length, then increase radius by ink inkIncrement
            if (_radius < _diagonal / 2)
                _radius += _increment;

            // Decrease alpha if it's not zero else reset animation variables
            if (_clickAlpha > 0)
            {
                _clickAlpha -= 1;
            }
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

            // Alpha
            if (Focused)
            {
                if (_accentAlpha < 255)
                    _accentAlpha += 15;
            }
            else
            {
                if (_accentAlpha > 0)
                    _accentAlpha -= 15;
            }

            // Turn off timer to safe some power
            // if (!_hover && _radius == 0 && _clickAlpha == 0 && _hoverAlpha == 0 && _accentAlpha == 0)
            //  Component.Outdated = false;

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

            // Graphics
            Graphics g = e.Graphics;

            // Clear control
            g.Clear(Parent.BackColor);

            // Client rectangle
            Rectangle r = ClientRectangle;

            // Draw raised button
            if (_buttonType == ButtonType.Raised)
            {
                // Fill color
                Color fill = new Color();

                // Button is filled with his Accent color
                if (_customColor)
                {
                    // If control is enabled
                    if (Enabled)
                    {
                        // Control enabled
                        fill = ColorExtensions.AddRGB((Component.SourceTheme.DARK_BASED) ? +(_hoverAlpha / 15) : -(_hoverAlpha / 15), Component.Accent);
                    }
                    else
                    {
                        // Control disabled
                        fill = Component.SourceTheme.COMPONENT_FILL.Disabled;
                    }

                    // Fill client rectangle with correct color (background layer)
                    using (SolidBrush brush = new SolidBrush(fill))
                        g.FillRectangle(brush, ClientRectangle);

                    // Turn on anti-aliasing
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    // Mouse position relative to parent form
                    Point mouse = PointToClient(Cursor.Position);

                    // Draw reveal effect
                    // Reveal effect ellipse
                    GraphicsPath ellipse = Draw.GetEllipsePath(mouse, _revealDistance);

                    // Path gradient brush
                    PathGradientBrush pgb = new PathGradientBrush(ellipse);
                    pgb.CenterPoint = mouse;
                    pgb.CenterColor = ColorExtensions.AddRGB(Component.SourceTheme.DARK_BASED ? 70 : -70, Accent);
                    pgb.SurroundColors = new Color[] { ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, fill), fill) };

                    // Draw reveal effect if mouse is hovering over an item
                    if (!DesignMode)
                        g.FillPath(pgb, ellipse);

                    // Turn off anti-aliasing
                    g.SmoothingMode = SmoothingMode.Default;

                    // Shortcut variable for client rectangle
                    Rectangle cr = ClientRectangle;

                    // Fill client rectangle with correct color (foreground layer)
                    using (SolidBrush brush = new SolidBrush(fill)) 
                        g.FillRectangle(brush, new Rectangle(cr.X + 2, cr.Y + 2, cr.Width - 4, cr.Height - 4));
                }
                else
                {
                    // Regular raised button
                    fill = ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, Component.SourceTheme.COMPONENT_FILL.Hover), Component.SourceTheme.COMPONENT_FILL.Normal.ToColor());

                    // Control is disabled
                    if (!Enabled)
                    {
                        // Change fill color
                        fill = Component.SourceTheme.COMPONENT_FILL.Disabled;
                    }

                    // Fill client rectangle with correct color (background layer)
                    using (SolidBrush brush = new SolidBrush(fill))
                        g.FillRectangle(brush, ClientRectangle);

                    // Turn on anti-aliasing
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    // Mouse position relative to parent form
                    Point mouse = PointToClient(Cursor.Position);

                    // Draw reveal effect
                    // Reveal effect ellipse
                    GraphicsPath ellipse = Draw.GetEllipsePath(mouse, _revealDistance);

                    // Path gradient brush
                    PathGradientBrush pgb = new PathGradientBrush(ellipse);
                    pgb.CenterPoint = mouse;
                    pgb.CenterColor = Component.SourceTheme.COMPONENT_HIGHLIGHT.Normal.ToColor();
                    pgb.SurroundColors = new Color[] { ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, fill), fill) };

                    // Draw reveal effect if mouse is hovering over an item
                    if (!DesignMode)
                        g.FillPath(pgb, ellipse);

                    // Turn off anti-aliasing
                    g.SmoothingMode = SmoothingMode.Default;

                    // Shortcut variable for client rectangle
                    Rectangle cr = ClientRectangle;

                    // Fill client rectangle with correct color (foreground layer)
                    using (SolidBrush brush = new SolidBrush(fill))
                        g.FillRectangle(brush, new Rectangle(cr.X + 2, cr.Y + 2, cr.Width - 4, cr.Height - 4));
                }

                // Draw click-effect
                DrawClick(e);

                // Draws text
                DrawText(e);
            }

            // Draws outlined button
            if (_buttonType == ButtonType.Outline)
            {
                // Fill color
                Color fill = new Color();

                // Border color
                Color border = new Color();

                // Button is filled with his Accent color
                if (_customColor)
                {
                    // Full-colored outlined button
                    fill = ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, ColorExtensions.Fade(220, Component.Accent, Parent.BackColor)), Parent.BackColor);
                    border = (Enabled) ? Component.Accent : Component.SourceTheme.COMPONENT_FILL.Disabled.ToColor();

                    // Control is not enabled
                    if (!Enabled)
                    {
                        fill = Parent.BackColor;
                    }
                }
                else
                {
                    // Full colored outlined button
                    fill = ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, ColorExtensions.Fade(220, Component.SourceTheme.COMPONENT_BORDER.Normal, Parent.BackColor)), Parent.BackColor);
                    border = (Enabled) ? Component.SourceTheme.COMPONENT_FILL.Normal.ToColor() : Component.SourceTheme.COMPONENT_FILL.Disabled.ToColor();

                    // Control is not enabled
                    if (!Enabled)
                    {
                        fill = Parent.BackColor;
                    }
                }

                // Fills client rectangle
                using (SolidBrush brush = new SolidBrush(border))
                    g.FillRectangle(brush, r);

                // Turn on anti-aliasing
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Mouse position relative to parent form
                Point mouse = PointToClient(Cursor.Position);

                // Draw reveal effect
                // Reveal effect ellipse
                GraphicsPath ellipse = Draw.GetEllipsePath(mouse, _revealDistance);

                // Path gradient brush
                PathGradientBrush pgb = new PathGradientBrush(ellipse);
                pgb.CenterPoint = mouse;
                pgb.CenterColor = (CustomColor) ? ColorExtensions.AddRGB(Component.SourceTheme.DARK_BASED ? 70 : -70, Accent) : Component.SourceTheme.COMPONENT_HIGHLIGHT.Normal.ToColor();
                pgb.SurroundColors = new Color[] { ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, border), border) };

                // Draw reveal effect if mouse is hovering over an item
                if (!DesignMode)
                    g.FillPath(pgb, ellipse);

                // Turn off anti-aliasing
                g.SmoothingMode = SmoothingMode.Default;

                // Fills border (frame)
                using (SolidBrush pen = new SolidBrush(fill))
                    g.FillRectangle(pen, new Rectangle(r.X + 2, r.Y + 2, r.Width - 4, r.Height - 4));

                // Draws click effect
                DrawClick(e);

                // Text
                DrawText(e);
            }

            // Flat button
            if (_buttonType == ButtonType.Flat)
            {
                // Fill color
                Color fill = new Color();

                // Button is filled with his Accent color
                if (_customColor)
                {
                    // Full colored outlined button
                    fill = ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, ColorExtensions.Fade(220, Component.Accent, Parent.BackColor)), Parent.BackColor);

                    // Control is not enabled
                    if (!Enabled)
                    {
                        fill = Parent.BackColor;
                    }
                }
                else
                {
                    // Full colored outlined button
                    fill = ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, ColorExtensions.Fade(160, Component.SourceTheme.COMPONENT_BORDER.Normal, Parent.BackColor)), Parent.BackColor);

                    // Control is not enabled
                    if (!Enabled)
                    {
                        fill = Parent.BackColor;
                    }
                }

                // Fills rectangle (background layer)
                using (SolidBrush brush = new SolidBrush(fill))
                    g.FillRectangle(brush, r);

                // Turn on anti-aliasing
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Mouse position relative to parent form
                Point mouse = PointToClient(Cursor.Position);

                // Draw reveal effect
                // Reveal effect ellipse
                GraphicsPath ellipse = Draw.GetEllipsePath(mouse, _revealDistance);

                // Path gradient brush
                PathGradientBrush pgb = new PathGradientBrush(ellipse);
                pgb.CenterPoint = mouse;
                pgb.CenterColor = (CustomColor) ? Color.FromArgb(255, Accent) : Component.SourceTheme.COMPONENT_HIGHLIGHT.Normal.ToColor();
                pgb.SurroundColors = new Color[] { ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, fill), fill) };

                // Draw reveal effect if mouse is hovering over an item
                if (!DesignMode)
                    g.FillPath(pgb, ellipse);

                // Turn off anti-aliasing
                g.SmoothingMode = SmoothingMode.Default;

                // Shortcut variable for client rectangle
                Rectangle cr = ClientRectangle;

                // Fills rectangle (foreground layer)
                using (SolidBrush brush = new SolidBrush(fill))
                    g.FillRectangle(brush, new Rectangle(cr.X + 2, cr.Y + 2, cr.Width - 4, cr.Height - 4));

                // Draws click-effect
                DrawClick(e);

                // Draws text
                DrawText(e);
            }
        }

        /// <summary>
        /// Draw click effect
        /// </summary>
        private void DrawClick(PaintEventArgs e)
        {
            // Control's graphics object
            Graphics g = e.Graphics;

            // ClickEffect
            if (_clickEffect != ClickEffect.None)
            {
                // Color of click effect
                Color color;
                Color fill = (_customColor) ? Component.Accent : Component.SourceTheme.COMPONENT_FILL.Normal.ToColor();

                if (Component.SourceTheme.DARK_BASED == true)
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
                if (_clickEffect == ClickEffect.Ripple)
                {
                    // Ink's brush and graphics path
                    GraphicsPath ink = Draw.GetEllipsePath(_mouseClickPosition, (int)_radius);

                    // Draws ripple click effect
                    using (SolidBrush brush = new SolidBrush(color))
                        g.FillPath(brush, ink);
                }

                // Square
                if (_clickEffect == ClickEffect.Square || _clickEffect == ClickEffect.SquareRotate)
                {
                    // Square's brush and graphics path
                    GraphicsPath square = Draw.GetSquarePath(_mouseClickPosition, (int)_radius);

                    // Rotates square
                    if (_clickEffect == ClickEffect.SquareRotate)
                    {
                        Matrix matrix = new Matrix();
                        matrix.RotateAt(_rotation, _mouseClickPosition);
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
        /// Draw text
        /// </summary>
        private void DrawText(PaintEventArgs e)
        {
            // Control's graphics object
            Graphics g = e.Graphics;

            // Converts alignment types
            StringFormat format = new StringFormat();
            int lNum = (int)Math.Log((double)TextAlign, 2);
            format.LineAlignment = (StringAlignment)(lNum / 4);
            format.Alignment = (StringAlignment)(lNum % 4);

            // Client Rectangle
            int offset = 10;
            Rectangle c = ClientRectangle;
            Rectangle r = new Rectangle(c.X + offset, c.Y + offset, c.Width - (offset * 2), c.Height - (offset * 2));

            // Text color
            Color color = Component.SourceTheme.COMPONENT_FOREGROUND.Normal;

            // Modify color based on chosen button type
            if (!Enabled)
            {
                // Control not enabled
                color = Component.SourceTheme.COMPONENT_FOREGROUND.Disabled;
            }
            else
            {
                // Control enabled
                color = Component.SourceTheme.COMPONENT_FOREGROUND.Normal;

                // Full colored button, but not outlined
                if (_customColor && _buttonType != ButtonType.Outline)
                    color = ForeColor;

                // Full colored button, but outlined
                if (_buttonType == ButtonType.Outline && _customColor)
                    color = Component.Accent;

                // Full colored and flat button
                if (_buttonType == ButtonType.Flat && _customColor)
                    color = Component.Accent;
            }

            // Draw text
            using (SolidBrush brush = new SolidBrush(color))
                g.DrawString((_capitalText) ? Text.ToUpper() : Text, Font, brush, r, format);
        }

        /// <summary>
        /// Occurs when control gets focus.
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            // Calls base method
            base.OnGotFocus(e);

            // Turn on timer
            Component.Outdated = true;
        }

        /// <summary>
        /// OnMouseEnter method. Check if mouse is inside of button
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            // Calls base method
            base.OnMouseEnter(e);

            // Update hover
            _hover = true;

            // Control needs to be updated
            Component.Outdated = true;
        }

        /// <summary>
        /// OnMouseLeave method. Check if mouse is outside of button
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            // Calls base method
            base.OnMouseLeave(e);

            // Update hover
            _hover = false;
        }

        /// <summary>
        /// OnMouseMove method. Triggered when user moves with mouse.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Calls base method
            base.OnMouseMove(e);

            // Update mouse position
            _mouse = e.Location;
        }

        /// <summary>
        /// Control resize event
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            // Calls base method
            base.OnResize(e);

            // Update diagonal
            _diagonal = (int)(2 * Math.Sqrt(Math.Pow(Width, 2) + Math.Pow(Height, 2)));

            // Turn on timer
            Component.Outdated = true;
        }

        /// <summary>
        /// Control click event
        /// </summary>
        protected override void OnClick(EventArgs e)
        {
            // Calls base method
            base.OnClick(e);

            // Catch if button was disposed
            try
            {
                // Update mouse position
                _mouseClickPosition = PointToClient(MousePosition);
            }
            catch { }

            // Updates click-effects
            // Ink
            if (_clickEffect == ClickEffect.Ripple)
                _radius = Width / 5;

            // Square
            if (_clickEffect == ClickEffect.Square || _clickEffect == ClickEffect.SquareRotate)
                _radius = Width / 8;

            // Resets alpha
            _clickAlpha = 25;
        }

        /// <summary>
        /// Change service
        /// </summary>
        private IComponentChangeService _changeService;

        /// <summary>
        /// Overrides site. Renames button at design time.
        /// </summary>
        public override ISite Site
        {
            get
            {
                // Return site
                return base.Site;
            }
            set
            {
                // Change service
                _changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

                // If change service is not null
                if (_changeService != null)
                {
                    _changeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                }

                // Update site property
                base.Site = value;

                // If we are not in design mode
                if (!DesignMode)
                {
                    return;
                }

                // Update change service
                _changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

                // If changeService is not null
                if (_changeService != null)
                {
                    // Set new event handler
                    _changeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
                }
            }
        }

        /// <summary>
        /// Fires when component is changed. This event triggers before
        /// control is placed in to the designer.
        /// </summary>
        private void OnComponentChanged(object sender, ComponentChangedEventArgs ce)
        {
            // Cast component to MButton
            MButton aBtn = ce.Component as MButton;

            // If cast was not successful or we are not in design mode
            if (aBtn == null || !aBtn.DesignMode)
                return;

            // If site or member is null and name is not "Text"
            if (((IComponent)ce.Component).Site == null || ce.Member == null || ce.Member.Name != "Text")
                return;

            // Renames button
            if (aBtn.Text == aBtn.Name)
                aBtn.Text = aBtn.Name.Replace("mButton", "button");
        }
    }
}
