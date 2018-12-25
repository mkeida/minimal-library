using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Minimal.Components;
using Minimal.Components.Core;
using Minimal.Components.Items;
using Minimal.External;
using Minimal.Themes;

namespace Minimal.Components
{
    /// <summary>
    /// Context-menu class
    /// </summary>
    public class MContextMenu : IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        public MComponent Component { get; set; }

        /// <summary>
        /// Context-menu popup form
        /// </summary>
        private ContextForm _context = new ContextForm();

        /// <summary>
        /// Inner list-box
        /// </summary>
        private MListBox _listBox = new MListBox();

        /// <summary>
        /// Items
        /// </summary>
        private ObservableCollection<MItem> _items = new ObservableCollection<MItem>();

        /// <summary>
        /// Items collection
        /// </summary>
        [Category("Minimal")]
        [Description("Collection of list-box's items.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObservableCollection<MItem> Items
        {
            get { return _items; }
            set
            {
                // Update items variable
                _items = value;
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
                    Component.Outdated = true;
                }
            }
        }

        /// <summary>
        /// Additional position increment;
        /// </summary>
        private int _positionOffset = 0;

        /// <summary>
        /// Opacity of context-menu form
        /// </summary>
        private float _opacityLevel = 0;

        /// <summary>
        /// Length of popup's animation travel
        /// </summary>
        private int _positionTravelLength = 15;

        /// <summary>
        /// Position of context menu
        /// </summary>
        private Point _position;

        /// <summary>
        /// Constructor
        /// </summary>
        public MContextMenu(Form owner)
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Update form owner
            Component.ParentForm = owner;

            // Configure list-box
            _listBox.Location = new Point(1, 1);
            _listBox.Size = new Size(_context.Width - 2, _context.Height - 2);
            _listBox.Accent = Accent;
            _listBox.SelectedItemChanged += new PropertyChangedEventHandler(ListBoxSelectedItemChanged);

            // Configure context-menu
            _context.Size = new Size(400, 250);
            _context.BorderColor = Accent;
            _context.SizeChanged += new EventHandler(OnContextMenuSizeChanged);

            // Background color
            _context.BackColor = _listBox.Accent;

            // Add list-box to panel
            _context.Controls.Add(_listBox);

            // Assign collection changed event for items collection
            Items.CollectionChanged += ItemsChanged;

            // Hook up update method
            Component.ComponentUpdate += UpdateComponent;

            // Hoop up theme changed method
            Component.ThemeChanged += ThemeChanged;
        }

        /// <summary>
        /// Update method
        /// </summary>
        private void UpdateComponent(object sender, EventArgs e)
        {
            // Update accent
            _listBox.Accent = Accent;
            _context.BackColor = Accent;

            // Updates position offset
            if (_positionOffset < _positionTravelLength)
                _positionOffset += 1;

            // Updates opacity level
            if (_opacityLevel < 1f)
                _opacityLevel += 0.05f;

            // Updates position
            _context.Location = new Point(_context.Location.X, _position.Y - _positionOffset);

            // Updates opacity
            _context.Opacity = _opacityLevel;
        }

        /// <summary>
        /// Items changed method
        /// </summary>
        private void ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Update items of inner list-box
            _listBox.Items = Items;
        }

        /// <summary>
        /// Context-menu size changed
        /// </summary>
        private void OnContextMenuSizeChanged(object sender, EventArgs e)
        {
            // Updates inner list-box location and size
            _listBox.Location = new Point(1, 1);
            _listBox.Size = new Size(_context.Width - 2, _context.Height - 2);
        }

        /// <summary>
        /// List-box's selected item changed
        /// </summary>
        private void ListBoxSelectedItemChanged(object sender, EventArgs e)
        {
            // Hides context-menu
            _context.Hide();
        }

        /// <summary>
        /// Theme changed method
        /// </summary>
        private void ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            // Update theme
            _listBox.Component.SourceTheme = e.Theme;

            // Update border color
            _context.BorderColor = Accent;
        }

        /// <summary>
        /// Shows context-menu
        /// </summary>
        public void Show(Point position)
        {
            // Resets position offset
            _positionOffset = 0;

            // Resets opacity level
            _opacityLevel = 0f;

            // Resets selected item
            _listBox.SelectedItem = null;

            // Check if context-menu have enough space
            if (position.X + _context.Width < Screen.FromControl(Component.ParentForm).Bounds.Width)
                // Set position
                _context.Location = position;
            else
                // Not enough space. Displays context-menu of left side of given point
                _context.Location = new Point(position.X - _context.Width, position.Y);

            // Updates length of context-menu
            // Temporary length variable
            int tempItemsLength = 0;

            // Calculate total length of all items in list-box
            foreach (MItem item in _listBox.Items)
            {
                // Trigger draw item to update item's Height property
                item.TriggerDrawItem(_listBox.CreateGraphics(), new Rectangle());

                // Increment total item height
                tempItemsLength += item.Height;
            }

            // Check if such long context-menu can fit on the screen
            if (position.Y + tempItemsLength < Screen.FromControl(Component.ParentForm).Bounds.Height)
            {
                // Context-menu is small enough to fit on screen
                // Hide scroll-bar
                _listBox.Scrollbar.Visible = false;

                // Update context-menu height
                _context.Height = tempItemsLength + 2;
            }
            else
            {
                // Menu can not be displayed under the mouse position.
                // Can it be displayed above mouse position?
                if (position.Y - tempItemsLength > 0)
                {
                    // Context-menu is small enough to fit on screen
                    // Hide scroll-bar
                    _listBox.Scrollbar.Visible = false;

                    // Update context-menu height
                    _context.Height = tempItemsLength + 2;

                    // Update position
                    _context.Location = new Point(_context.Location.X, _context.Location.Y - _context.Height);
                }
                else
                {
                    // Show scroll-bar
                    _listBox.Scrollbar.Visible = true;

                    // Update context-menu location
                    _context.Location = new Point(_context.Location.X, position.Y);

                    // Update context-menu height
                    _context.Height = 200;
                }
            }

            // Update position
            _position = new Point(_context.Location.X, _context.Location.Y + _positionTravelLength);

            // Brings form to front
            _context.BringToFront();

            // Redraw list.box
            _listBox.Refresh();

            // Shows window
            _context.Show();
        }

        /// <summary>
        /// Hides context-menu
        /// </summary>
        public void Hide()
        {
            // Hides popup form
            _context.Hide();
        }

        /// <summary>
        /// Returns true, if component needs to be updated
        /// </summary>
        public bool IsOutdated()
        {
            return true;
        }
    }
}
