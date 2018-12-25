using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Minimal.Components.Core;
using Minimal.Themes;

namespace Minimal.Components
{
    /// <summary>
    /// Scroll-able panel (currently supports only vertical scroll)
    /// </summary>
    public partial class MScrollablePanel : UserControl, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        public MComponent Component { get; set; }

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
        /// Panel
        /// </summary>
        [Category("Minimal")]
        [Description("Inner panel.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MBufferedPanel Panel { get; set; }

        /// <summary>
        /// Scrollbar
        /// </summary>
        [Category("Minimal")]
        [Description("Inner scroll-bar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MVerticalScrollbar Scrollbar { get; set; }

        /// <summary>
        /// Scrollbar visible
        /// </summary>
        private bool _scrollbarVisible = true;

        /// <summary>
        /// Scrollbar visible changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the ScrollbarVisible is changed.")]
        public event PropertyChangedEventHandler ScrollbarVisibleChanged;

        /// <summary>
        /// Scrollbar visible property
        /// </summary>
        public bool ScrollbarVisible
        {
            get { return _scrollbarVisible; }
            set
            {
                _scrollbarVisible = value;
                ScrollbarVisibleChanged?.Invoke(this, new PropertyChangedEventArgs("ScrollbarVisibleChanged"));

                if (_scrollbarVisible)
                {
                    Controls.Add(Scrollbar);
                }
                else
                {
                    Controls.Remove(Scrollbar);
                }
            }
        }

        /// <summary>
        /// Y offset right after touch
        /// </summary>
        private int _touchStartY;

        /// <summary>
        /// Y offset after touch
        /// </summary>
        private int _touchEndY;

        /// <summary>
        /// Difference between touch-start-Y and touch-end-Y
        /// </summary>
        private int scrollAmount;

        /// <summary>
        /// True if left mouse button is down
        /// </summary>
        private bool mouseDown = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public MScrollablePanel()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialize control
            InitializeComponent();

            // Default variables
            Panel = new MBufferedPanel();
            Panel.Dock = DockStyle.Fill;
            Panel.AutoScroll = true;
            Panel.Scroll += new ScrollEventHandler(PanelScroll);
            Panel.MouseWheel += new MouseEventHandler(PanelWheelScroll);
            Panel.AutoScrollPosition = new Point(0, 0);
            Panel.MouseDown += new MouseEventHandler(OnPanelMouseDown);
            Panel.MouseUp += new MouseEventHandler(OnPanelMouseUp);
            Panel.MouseMove += new MouseEventHandler(OnPanelMouseMove);
            Scrollbar = new MVerticalScrollbar();
            Scrollbar.Dock = DockStyle.Right;
            Scrollbar.Scroll += new EventHandler(ScrollbarScroll);

            // Adds panel and scrollbar
            Controls.Add(Panel);
            Controls.Add(Scrollbar);

            // Redraw control
            Invalidate(true);
        }

        /// <summary>
        /// Mouse down event
        /// </summary>
        private void OnPanelMouseDown(object sender, MouseEventArgs e)
        {
            // Y position of mouse on mouse-down event
            _touchStartY = e.Location.Y;

            // Mouse button is down
            mouseDown = true;
        }

        /// <summary>
        /// Mouse up event
        /// </summary>
        private void OnPanelMouseUp(object sender, MouseEventArgs e)
        {
            // Mouse button is up
            mouseDown = false;
        }

        /// <summary>
        /// Panel mouse move event
        /// </summary>
        private void OnPanelMouseMove(object sender, MouseEventArgs e)
        {
            // Is touch enabled?
            if (M.TouchEnabled)
            {
                // Y position of mouse
                _touchEndY = e.Location.Y;

                // Is mouse down?
                // Eventually increments scroll-amount
                if (mouseDown)
                    scrollAmount = _touchEndY - _touchStartY;
                else
                    scrollAmount = 0;

                // Updates scroll-bar value
                Scrollbar.Value -= scrollAmount * 2;

                // Resets touch start
                _touchStartY = e.Location.Y;
            }
        }

        /// <summary>
        /// Draw method
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base call
            base.OnPaint(e);

            // Update colors
            if (Component != null)
            {
                Panel.BackColor = Component.SourceTheme.COMPONENT_BACKGROUND.Normal.ToColor();
                BackColor = Component.SourceTheme.COMPONENT_BACKGROUND.Normal.ToColor();
            }
        }

        /// <summary>
        /// Panel mouse wheel scroll
        /// </summary>
        private void PanelWheelScroll(object sender, MouseEventArgs e)
        {
            // Updates scroll-bar value
            Scrollbar.Value = Math.Abs(Panel.AutoScrollPosition.Y);
        }

        /// <summary>
        /// Panel default scrollbar scroll
        /// </summary>
        private void PanelScroll(object sender, EventArgs e)
        {
            // Updates scroll-bar value
            Scrollbar.Value = Math.Abs(Panel.AutoScrollPosition.Y);
        }

        /// <summary>
        /// Scrollbar scroll
        /// </summary>
        private void ScrollbarScroll(object sender, EventArgs e)
        {
            // Updates panel auto-scroll position
            Panel.AutoScrollPosition = new Point(0, Scrollbar.Value);
        }

        /// <summary>
        /// Resize control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            // Base call
            base.OnResize(e);

            // Update scroll-bar's variables
            Scrollbar.Minimum = 0;
            Scrollbar.Maximum = Panel.DisplayRectangle.Height - Panel.Size.Height;

            // Calculate cursor length
            CalculateCursorLength();
        }

        /// <summary>
        /// Calculate cursor length
        /// </summary>
        private void CalculateCursorLength()
        {
            // Calculate cursor length
            if (Panel.DisplayRectangle.Height > 0)
            {
                // Help variables
                decimal viewableRatio = Convert.ToDecimal(Panel.Size.Height) / Convert.ToDecimal(Panel.DisplayRectangle.Height);
                decimal scrollBarArea = Scrollbar.Height;
                decimal thumbHeight = scrollBarArea * viewableRatio;

                // Update scroll-bar's cursor (scroll-thumb) length
                Scrollbar.CursorLength = Convert.ToInt32(thumbHeight);
            }
        }

        /// <summary>
        /// Triggers when input type is changed
        /// </summary>
        public void InputTypeChanged(bool touchEnabled)
        {
            // Touch input is enabled
            if (touchEnabled)
            {
                // Makes scroll-bar smaller
                Scrollbar.Minified = true;

                // Turn on auto-hide
                Scrollbar.AutoHide = true;
            }
            else
            {
                // Makes scroll-bar bigger
                Scrollbar.Minified = false;

                // Turn off auto-hide
                Scrollbar.AutoHide = false;
            }
        }
    }
}
