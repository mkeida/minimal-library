using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Minimal.Components.Items;
using Minimal.Internal;
using Minimal.External;
using Minimal.Themes;
using Minimal.Components.Core;

namespace Minimal.Components
{
    /// <summary>
    /// Combo-box control
    /// </summary>
    [Designer("Minimal.Components.Designer.ComboBoxDesigner")]
    [DefaultEvent("SelectedItemChanged")]
    public partial class MComboBox : MBufferedPanel, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        public MComponent Component { get; set; }

        /// <summary>
        /// Default text variable
        /// </summary>
        private string _defaultText = "ComboBox";

        /// <summary>
        /// Default text
        /// </summary>
        [Category("Minimal")]
        [Description("Default text of the combo-box when no item is selected.")]
        public string DefaultText
        {
            get { return _defaultText; }
            set
            {
                // Updates default-text
                _defaultText = value;

                // Updates header text
                _header.Text = value;
            }
        }

        /// <summary>
        /// Items property
        /// </summary>
        [Category("Minimal")]
        [Description("Combo-box's items.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObservableCollection<MItem> Items { get; set; } = new ObservableCollection<MItem>();

        /// <summary>
        /// Default click effect
        /// </summary>
        private ClickEffect _clickEffect = ClickEffect.Ripple;

        /// <summary>
        /// Button ClickEffect changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when the click effect type is changed.")]
        public event PropertyChangedEventHandler ClickEffectChanged;

        /// <summary>
        /// ClickEffect property
        /// </summary>
        [Category("Minimal")]
        [Description("Click effect of the combo-box.")]
        public ClickEffect ClickEffect
        {
            get { return _clickEffect; }
            set
            {
                if (value != _clickEffect)
                {
                    // Update click-effect value
                    _clickEffect = value;

                    // Trigger changed event
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

                    // Update inner list-box accent
                    _listBox.Accent = value;

                    // Update search-field list-box accent
                    _search.Accent = value;

                    // Invoke property changed event handler
                    AccentChanged?.Invoke(this, new PropertyChangedEventArgs("AccentChanged"));

                    // Redraw component
                    Invalidate(true);
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
        /// Display search-bar local variable
        /// </summary>
        private bool _displaySearchbar;

        /// <summary>
        /// DisplaySearch changed event
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when DisplaySearchbar is changed.")]
        public event PropertyChangedEventHandler DisplaySearchChanged;

        /// <summary>
        /// Display search-bar property
        /// </summary>
        [Category("Minimal")]
        [Description("True if combo-box should have search-bar.")]
        public bool DisplaySearch
        {
            get { return _displaySearchbar; }
            set
            {
                // Change used theme
                _displaySearchbar = value;

                // Fire event
                DisplaySearchChanged?.Invoke(this, new PropertyChangedEventArgs("DisplaySearchChanged"));

                // Is search-bar enabled?
                if (_displaySearchbar)
                    _container.Controls.Add(_search);
                else
                    _container.Controls.Remove(_search);

                // Redraw control
                Invalidate(true);
            }
        }

        /// <summary>
        /// Search-bar text local variable
        /// </summary>
        private string _searchBarText = "Search...";

        /// <summary>
        /// Search bat text property
        /// </summary>
        [Category("Minimal")]
        [Description("Hint text of the search-bar.")]
        public string SearchBarText
        {
            get
            {
                // Return our private variable
                return _searchBarText;
            }
            set
            {
                // Set new value
                _searchBarText = value;

                // Update Text property of text-box
                _search.LeftHint = value;
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
        public bool ScrollbarAutoHide
        {
            get { return _autoHide; }
            set
            {
                if (value != _autoHide)
                {
                    // Update auto-hide value
                    _autoHide = value;

                    // Trigger changed event
                    AutoHideChanged?.Invoke(this, new PropertyChangedEventArgs("AutoHideChanged"));

                    // Modify scroll-bar
                    _listBox.Scrollbar.AutoHide = value;
                }

                // Redraw
                Invalidate();
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
        public event PropertyChangedEventHandler ScrollbarMinifiedChanged;

        /// <summary>
        /// Minified property
        /// </summary>
        [Category("Minimal")]
        [Description("True if scrollbar should be minified.")]
        public bool ScrollbarMinified
        {
            get { return _minified; }
            set
            {
                // Change used theme
                _minified = value;

                // Fire event
                ScrollbarMinifiedChanged?.Invoke(this, new PropertyChangedEventArgs("MinifiedChanged"));

                // Modify scroll-bar
                _listBox.Scrollbar.Minified = value;

                // Redraw control
                Invalidate(true);
            }
        }

        /// <summary>
        /// Selected item
        /// </summary>
        public MItem SelectedItem = null;

        /// <summary>
        /// Selected item changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when SelectedItem is changed.")]
        public event PropertyChangedEventHandler SelectedItemChanged;

        /// <summary>
        /// ComboBox head
        /// </summary>
        private MComboBoxHeader _header;

        /// <summary>
        /// ListBox for Items
        /// </summary>
        private MListBox _listBox;

        /// <summary>
        /// Search-bar
        /// </summary>
        private MTextBox _search;

        /// <summary>
        /// Container
        /// </summary>
        private Panel _container;

        /// <summary>
        /// True if comboBox is opened
        /// </summary>
        private bool _opened;

        /// <summary>
        /// Animation time in milliseconds
        /// </summary>
        private double _t;

        /// <summary>
        /// Drop-down length
        /// </summary>
        private int _dropdownLength;

        /// <summary>
        /// True if the mouse is inside of the control
        /// </summary>
        private bool _hover;

        /// <summary>
        /// Alpha value of the hover effect which is later added to control fill color
        /// </summary>
        private byte _hoverAlpha;

        /// <summary>
        /// Tint alpha
        /// </summary>
        private byte _tintAlpha;

        /// <summary>
        /// Temporary variable for saving scroll-bar value before it enters 0px height
        /// </summary>
        private int _tempScrollbarValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public MComboBox()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialize component
            InitializeComponent();

            // Default variables
            // Initialize combo-box's header
            _header = new MComboBoxHeader();
            _header.Height = 35;
            _header.Click += new EventHandler(HeaderClick);
            _header.Dock = DockStyle.Top;
            _header.Accent = Accent;
            _header.MouseEnter += new EventHandler(HeaderMouseEnter);
            _header.MouseLeave += new EventHandler(HeaderMouseLeave);
            _header.LostFocus += new EventHandler(FocusLost);

            // Initialize search-bar
            _search = new MTextBox();
            _search.Dock = DockStyle.Top;
            _search.drawLeftBorder = false;
            _search.drawRightBorder = false;
            _search.drawTopBorder = false;
            _search.drawBottomBorder = false;
            _search.TextChanged += SearchTextChanged;
            _search.AutoHideHints = true;

            // Initialize list-box
            _listBox = new MListBox();
            _listBox.Dock = DockStyle.Fill;
            _listBox.SelectedItemChanged += ListBoxSelectedItemChanged;
            _listBox.Accent = Accent;
            _listBox.LostFocus += new EventHandler(FocusLost);
            _listBox.Scrollbar.LostFocus += new EventHandler(FocusLost);

            // Default variables
            _opened = false;
            _t = 1;
            _dropdownLength = 200;
            Height = _header.Height;
            Padding = new Padding(1);
            Items.CollectionChanged += ItemsChanged;

            // Initialize container
            _container = new Panel();
            _container.Dock = DockStyle.Fill;

            // Add container and header to controls collection
            Controls.Add(_container);
            Controls.Add(_header);

            // Add list-box to container
            _container.Controls.Add(_listBox);

            // Redraw
            Invalidate(true);
        }

        /// <summary>
        /// Occurs when a handle is created for the control. Handles
        /// event hooking.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            // Base call
            base.OnHandleCreated(e);

            try
            {
                // Hook up update method
                Component.ComponentUpdate += OnComponentUpdate;

                // Register parent click method
                Component.ParentForm.Click += ParentFormClick;

                // Hook up theme changed
                Component.ThemeChanged += OnThemeChanged;

                // Set container background color
                _container.BackColor = Component.SourceTheme.COMPONENT_BACKGROUND.Normal;
            }
            catch
            {
                // Designer error
            }
        }

        /// <summary>
        /// On theme changed
        /// </summary>
        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            // Set container background color
            _container.BackColor = Component.SourceTheme.COMPONENT_BACKGROUND.Normal;
        }

        /// <summary>
        /// Parent click
        /// </summary>
        private void ParentFormClick(object sender, EventArgs e)
        {
            // Closes drop-down
            Close();

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Update
        /// </summary>
        private void OnComponentUpdate(object sender, EventArgs e)
        {
            // Update list-box's accent
            _listBox.Accent = Accent;

            // Increment t
            _t += 0.05;

            // Limit t
            if (_t > 1)
                _t = 1;
            else
                _listBox.Scrollbar.Value = _tempScrollbarValue;

            // Opening and closing
            if (_opened)
                Height = _header.Height + Animation.CosinusMotion(_t, _dropdownLength);
            else
                Height = _dropdownLength + _header.Height + 2 - Animation.CosinusMotion(_t, _dropdownLength);

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

            // Alpha
            if (Focused || _listBox.Focused || _header.Focused || _search.Focused || _search.InnerTextBox.Focused || _listBox.Scrollbar.Focused)
            {
                if (_tintAlpha < 255)
                    _tintAlpha += 15;
            }
            else
            {
                if (_tintAlpha > 0)
                    _tintAlpha -= 15;
            }

            // Update colors
            Color fill = ColorExtensions.Mix(Color.FromArgb(_hoverAlpha, Component.SourceTheme.COMPONENT_FILL.Hover.ToColor()), Component.SourceTheme .COMPONENT_FILL.Normal.ToColor());
            Color frameColor = ColorExtensions.Mix(Color.FromArgb(_tintAlpha, Component.Accent), fill);
            BackColor = frameColor;

            // Forces scroll-bar update
            _listBox.Scrollbar.ForceUpdate();

            // Do control need to be updated anymore?
            if ((_tintAlpha == 255 || _tintAlpha == 0) && (_hoverAlpha == 255 || _hoverAlpha == 0) && _t == 1)
                Component.Outdated = false;
        }

        /// <summary>
        /// Selected item changed
        /// </summary>
        private void ListBoxSelectedItemChanged(object sender, PropertyChangedEventArgs e)
        {
            // Is item null?
            if (SelectedItem == null)
                _header.Text = DefaultText;

            // Update selected-item
            SelectedItem = _listBox.SelectedItem;

            // Trigger selected-item changed event
            SelectedItemChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItemChanged"));

            // Update header text
            if (SelectedItem != null)
                _header.Text = _listBox.SelectedItem.PrimaryText;

            // Redraw header
            _header.Invalidate();

            // Close drop-down
            Close();

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Items changed
        /// </summary>
        private void ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Update inner list-box items
            _listBox.Items = new ObservableCollection<MItem>(Items);

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Handles searching
        /// </summary>
        private void SearchTextChanged(object sender, PropertyChangedEventArgs e)
        {
            // Filter list-box
            _listBox.Filter = _search.Text;

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Click event
        /// </summary>
        private void HeaderClick(object sender, EventArgs e)
        {
            // Reset t variable
            _t = 0;

            // Handles drop-down closing and opening
            if (_opened)
            {
                // Closes drop-down
                Close();
            }
            else
            {
                // Opens drop-down
                Open();
            }

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Header and listBox focus lost
        /// </summary>
        private void FocusLost(object sender, EventArgs e)
        {
            // Skips drop-down close if user clicked in inner search-bar
            if (_search.Focused || _search.InnerTextBox.Focused || _listBox.Scrollbar.Focused || _listBox.Focused || _listBox.Panel.Focused)
                return;

            // Close drop-down
            Close();

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// On lost focus
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            // Skips drop-down close if user clicked in inner search-bar
            if (_search.Focused || _search.InnerTextBox.Focused || _listBox.Scrollbar.Focused)
                return;

            // Base call
            base.OnLostFocus(e);

            // Close drop-down
            Close();

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Resize event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            // base call
            base.OnResize(e);

            // Updates header width
            _header.Width = Width - 2;

            // Update
            Component.Outdated = true;
        }


        /// <summary>
        /// OnMouseEnter method. Check if mouse is inside of button
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            // Base call
            base.OnMouseEnter(e);

            // Updates hover
            _hover = true;

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// OnMouseLeave method. Check if mouse is outside of button
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            // Base call
            base.OnMouseLeave(e);

            // Updates hover
            _hover = false;

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Header mouse enter
        /// </summary>
        private void HeaderMouseEnter(object sender, EventArgs e)
        {
            // Updates hover
            _hover = true;

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Header mouse leave
        /// </summary>
        private void HeaderMouseLeave(object sender, EventArgs e)
        {
            // Updates hover
            _hover = false;

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Opens drop-down
        /// </summary>
        public void Open()
        {
            // Is drop-down closed?
            if (!_opened)
            {
                // Brings control to front
                BringToFront();

                // Updates opened variable
                _opened = true;

                // Reset t
                _t = 0;

                // Resets searched text
                _search.Text = "";
            }

            // Update
            Component.Outdated = true;
        }

        /// <summary>
        /// Closes drop-down
        /// </summary>
        public void Close()
        {
            // Is drop-down opened?
            if (_opened)
            {
                // Saves scrollbar value
                _tempScrollbarValue = _listBox.Scrollbar.Value;

                // Updates opened variable
                _opened = false;

                // Reset t
                _t = 0;
            }

            // Update
            Component.Outdated = true;
        }
    }
}
