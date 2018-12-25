using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Minimal.External;
using Minimal.Themes;
using Minimal.Components.Core;
using Minimal.Internal;

namespace Minimal.Components
{
    /// <summary>
    /// Simple label control
    /// </summary>
    [Designer("Minimal.Components.Designer.LabelDesigner")]
    public partial class MLabel : Label, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        [Category("Minimal")]
        [Description("Handles life-cycle of the M-Component.")]
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
        /// Default type
        /// </summary>
        private LabelType _type;

        /// <summary>
        /// Type changed event
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the Type property is changed.")]
        public event PropertyChangedEventHandler TypeChanged;

        /// <summary>
        /// Type property
        /// </summary>
        [Category("Minimal")]
        [Description("Type of the label.")]
        public LabelType Type
        {
            get { return _type; }
            set
            {
                if (value != _type)
                {
                    _type = value;
                    TypeChanged?.Invoke(this, new PropertyChangedEventArgs("TypeChanged"));
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
        /// Constructor
        /// </summary>
        public MLabel()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialization
            InitializeComponent();

            // Default font
            Font = new Font("Segoe UI", 9);
        }

        /// <summary>
        /// Draw label
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base call
            base.OnPaint(e);

            // Get graphics variable
            Graphics g = e.Graphics;

            // Normal and alternate color
            Color standardTextColor = Component.SourceTheme.COMPONENT_FOREGROUND.Normal.ToColor();
            Color alternateTextColor = (Component.SourceTheme.DARK_BASED) ? ColorExtensions.AddRGB(-120, Component.SourceTheme.COMPONENT_FOREGROUND.Normal.ToColor()) : ColorExtensions.AddRGB(100, Component.SourceTheme.COMPONENT_FOREGROUND.Normal.ToColor());
            Color accentTextColor = Component.Accent;

            // Set foreground color
            switch (_type)
            {
                case LabelType.Standard:
                    ForeColor = standardTextColor;
                    break;
                case LabelType.Alternate:
                    ForeColor = alternateTextColor;
                    break;
                case LabelType.Accent:
                    ForeColor = accentTextColor;
                    break;
            }
        }
    }
}
