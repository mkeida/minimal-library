using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Minimal.Components.Core;
using Minimal.Internal;

namespace Minimal.Components
{
    /// <summary>
    /// Drawer control
    /// </summary>
    public partial class MDrawer : Panel, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        public MComponent Component { get; set; }

        /// <summary>
        /// Opened variable
        /// </summary>
        private bool _opened;

        /// <summary>
        /// Control variable of opening and closing animation
        /// </summary>
        private double _animation;

        /// <summary>
        /// Opened property. If true, drawer is opened and visible
        /// </summary>
        public bool Opened
        {
            get
            {
                // Return value
                return _opened;
            }
            set
            {
                // Is new value different?
                if (Opened != value)
                {
                    // Set new value
                    _opened = value;
                    _animation = 0;

                    // Is menu being opened?
                    if (_opened)
                    {
                        BringToFront();
                    }
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MDrawer()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialize component
            InitializeComponent();

            // Set drawer width
            Width = 300;

            // Set location
            Location = new Point(-300, 0);

            // Disable animation start
            _animation = 1;

            // Default background color
            BackColor = Color.White;
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

            // Hook up theme changed
            Component.ThemeChanged += OnThemeChanged;

            // Check for parent form
            if (Component.ParentForm != null)
            {
                // Register parent form click method
                Component.ParentForm.Click += ParentFormClick;

                // Register parent form resize event
                Component.ParentForm.Resize += OnParentFormResize;
            }

            // Update height
            Height = Component.ParentForm.ClientSize.Height;

            // Update location
            Location = new Point(0, 0);

            // Are we in design-mode?
            if (DesignMode)
            {
                Visible = false;
            }
        }

        /// <summary>
        /// On parent form resize
        /// </summary>
        private void OnParentFormResize(object sender, EventArgs e)
        {
            // Update height
            Height = Component.ParentForm.ClientSize.Height;
        }

        /// <summary>
        /// Update method
        /// </summary>
        private void UpdateComponent(object sender, EventArgs e)
        {
            // Progress animation
            _animation += (_animation < 1) ? 0.05 : 0;

            // Handles closing and opening animation
            if (Opened)
                Location = new Point(Animation.CosinusMotion(_animation, 300) - 300, Location.Y);
            else
                Location = new Point(Animation.CosinusMotion(_animation, -300), Location.Y);
        }

        /// <summary>
        /// Triggered when theme is changed
        /// </summary>
        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            // Set background color
            BackColor = e.Theme.COMPONENT_BACKGROUND.Normal;
        }

        /// <summary>
        /// On parent form click
        /// </summary>
        private void ParentFormClick(object sender, EventArgs e)
        {
            // Close menu
            Opened = false;
        }
    }
}
