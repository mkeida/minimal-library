using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Minimal.Components.Items;
using Minimal.Components.Core;
using Minimal.Themes;

namespace Minimal.Components
{
    /// <summary>
    /// List-box control
    /// </summary>
    [DefaultEvent("SelectedItemChanged")]
    [Designer("Minimal.Components.Designer.ListBoxDesigner")]
    public partial class MListBox : MScrollablePanel, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        [Category("Minimal")]
        [Description("Handles life-cycle of the M-Component.")]
        public MComponent Component { get; set; }

        /// <summary>
        /// Items
        /// </summary>
        private ObservableCollection<MItem> _items = new ObservableCollection<MItem>();

        /// <summary>
        /// Items property
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

                // Filter items
                FilterItems();

                // Redraw
                Invalidate(true);
            }
        }

        /// <summary>
        /// Filtered items
        /// </summary>
        [Category("Minimal")]
        [Description("Items displayed when Filter property is assigned.")]
        public ObservableCollection<MItem> DisplayedItems { get; }

        /// <summary>
        /// Selected item changed event handler
        /// </summary>
        [Category("Minimal")]
        [Description("Fires when SelectedItem property is changed.")]
        public event PropertyChangedEventHandler SelectedItemChanged;

        /// <summary>
        /// Selected item
        /// </summary>
        [Category("Minimal")]
        [Description("Selected item from the list-box.")]
        public MItem SelectedItem { get; set; } = null;

        /// <summary>
        /// User can select multiple items at once using CTRL key + click if enabled.
        /// </summary>
        public bool MultiSelect { get; set; }

        /// <summary>
        /// Selected items private variable
        /// </summary>
        private List<MItem> selectedItems = new List<MItem>();

        /// <summary>
        /// Selected items. Works only if MultiSelect property is true.
        /// </summary>
        [Category("Minimal")]
        [Description("List of all selected items of the list-box.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<MItem> SelectedItems
        {
            get
            {
                return selectedItems;
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
                    Invalidate(true);
                }
            }
        }

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
        [Description("Click effect of listBox.")]
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
        /// Filter
        /// </summary>
        private string _filter = "";

        /// <summary>
        /// Filter property
        /// </summary>
        [Category("Minimal")]
        [Description("Filters all items in the Items collection. DisplayedItems collection is then presented to the user.")]
        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;

                // Filter items
                FilterItems();
            }
        }

        /// <summary>
        /// Last item Y position
        /// </summary>
        private int _lastItemY;

        /// <summary>
        /// Temporary M-Item variable. Helps with triggering OnMouseEnter and
        /// OnMouseLeave methods of M-Items.
        /// </summary>
        private MItem _tempItem = null;

        /// <summary>
        /// Mouse position when left button is down
        /// </summary>
        private Point mouseDownPoint;

        /// <summary>
        /// Mouse position
        /// </summary>
        internal Point mouseLocation;

        /// <summary>
        /// Constructor
        /// </summary>
        public MListBox()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialize control
            InitializeComponent();

            // Handle events
            Panel.Scroll += new ScrollEventHandler(OnPanelScroll);
            Panel.Paint += new PaintEventHandler(OnPanelPaint);
            Panel.MouseMove += new MouseEventHandler(OnPanelMouseMove);
            Panel.MouseLeave += new EventHandler(OnPanelMouseLeave);
            Panel.MouseClick += new MouseEventHandler(OnPanelClick);
            Panel.MouseDown += new MouseEventHandler(OnPanelMouseDown);
            Panel.Resize += new EventHandler(OnPanelResize);

            // Private variables and properties
            DisplayedItems = new ObservableCollection<MItem>();
            mouseLocation = new Point(Cursor.Position.X - Panel.AutoScrollPosition.X, Cursor.Position.Y - Panel.AutoScrollPosition.Y);
            _lastItemY = 0;

            // Assign collection changed event for items collection
            Items.CollectionChanged += ItemsChanged;
            DisplayedItems.CollectionChanged += DisplayedItemsChanged;

            // Redraw
            Panel.Invalidate();
        }

        /// <summary>
        /// Occurs when a handle is created for the control. Handles
        /// event hooking.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            // Base call
            base.OnHandleCreated(e);

            // Update items owner
            foreach (MItem item in Items)
            {
                item.Owner = this;
            }

            // Hook up update method
            Component.ComponentUpdate += UpdateComponent;

            // Update background color
            Panel.BackColor = Component.SourceTheme.COMPONENT_BACKGROUND.Normal;
        }

        /// <summary>
        /// Update method
        /// </summary>
        private void UpdateComponent(object sender, EventArgs e)
        {
            // Iterate over all visible items
            foreach (MItem item in DisplayedItems)
            {
                // Update item
                item.TriggerUpdate(this, e);
            }

            // Redraw
            Invalidate(true);
        }

        /// <summary>
        /// Mouse down event
        /// </summary>
        private void OnPanelMouseDown(object sender, MouseEventArgs e)
        {
            // Updates position of mouse on mouse-down event
            mouseDownPoint = e.Location;
        }

        /// <summary>
        /// Items changed
        /// </summary>
        private void ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Update items owner
            foreach (MItem item in Items)
            {
                item.Owner = this;
            }

            // Filter items
            FilterItems();

            // Redraw control
            Invalidate(true);

            // Force update component
            Component.Outdated = true;

            // Force update scroll-bar
            Scrollbar.ForceUpdate();

            // Call resize
            OnResize(EventArgs.Empty);

            // Refresh control
            Refresh();
        }

        /// <summary>
        /// Occurs where displayed items collection is changed
        /// </summary>
        private void DisplayedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // If there are no items visible
            if (DisplayedItems.Count == 0)
                Scrollbar.Visible = false;
            else
                Scrollbar.Visible = true;
        }

        /// <summary>
        /// Panel paint
        /// </summary>
        private void OnPanelPaint(object sender, PaintEventArgs e)
        {
            // Transform panel
            e.Graphics.TranslateTransform(Panel.AutoScrollPosition.X, Panel.AutoScrollPosition.Y);

            // Draw items
            DrawItems(e.Graphics);

            // Update background color
            Panel.BackColor = Component.SourceTheme.COMPONENT_BACKGROUND.Normal;
        }

        /// <summary>
        /// Draw items
        /// </summary>
        private void DrawItems(Graphics g)
        {
            // Y position if item
            int y = 0;

            // Do we have more than one item?
            if (Items.Count > 0)
            {
                // Iterates over all displayed-items
                foreach (MItem item in DisplayedItems)
                {
                    // Set item bounds
                    item.Bounds = new Rectangle(0, y, Panel.Width, item.Height);

                    // Draw item
                    item.TriggerDrawItem(g, item.Bounds);

                    // Increment last height
                    y += item.Height;
                }
            }

            // Set last item Y position
            _lastItemY = y;

            // Set minimum auto-scroll size
            Panel.AutoScrollMinSize = new Size(0, _lastItemY);

            // Call resize event
            OnResize(EventArgs.Empty);
        }

        /// <summary>
        /// Panel scroll
        /// </summary>
        private void OnPanelScroll(object sender, ScrollEventArgs e)
        {
            // Redraw panel
            Panel.Invalidate();
        }

        /// <summary>
        /// Panel click. Handles item selection.
        /// </summary>
        private void OnPanelClick(object sender, MouseEventArgs e)
        {
            // Is touch enabled?
            if (M.TouchEnabled)
            {
                // Cancels click event if click wasn't accurate enough
                // Ensures better scrolling experience
                if (mouseDownPoint != e.Location)
                    return;
            }

            // Get focus
            Focus();

            // Do we have more than one item?
            if (Items.Count > 0)
            {
                // Iterate over all displayed items
                foreach (MItem item in DisplayedItems)
                {
                    // Did user clicked on currently iterated item?
                    if (item.Bounds.Contains(mouseLocation))
                    {
                        // Modify mouse event arguments
                        MouseEventArgs clickArgs = new MouseEventArgs(
                            e.Button, 
                            e.Clicks, 
                            e.X, 
                            Math.Abs(Panel.AutoScrollPosition.Y) + e.Y,
                            e.Delta
                        );
                            
                        // Trigger clicked item click event
                        item.TriggerMouseClick(this, clickArgs);

                        // Did user left-clicked?
                        if (e.Button == MouseButtons.Left)
                        {
                            // Ignore if item is divider
                            if (item.Divider)
                                continue;

                            // Change selected item
                            if (MultiSelect)
                            {
                                // Is CTRL key pressed?
                                if ((ModifierKeys & Keys.Control) == Keys.Control)
                                {
                                    // Check if item was not selected before
                                    if (!SelectedItems.Contains(item))
                                    {
                                        // Add item to SelectedItems collection
                                        SelectedItems.Add(item);
                                    }
                                }
                                else
                                {
                                    // Clear items
                                    SelectedItems.Clear();

                                    // Add item to SelectedItems collection
                                    SelectedItems.Add(item);
                                }
                            }
                            else
                            {
                                // Update selected item
                                SelectedItem = item;
                            }

                            // Trigger selected item changed event
                            SelectedItemChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItemChanged"));
                        }
                    }
                }
            }

            // Redraw
            Invalidate(true);
        }

        /// <summary>
        /// Panel resize event
        /// </summary>
        private void OnPanelResize(object sender, EventArgs e)
        {
            // Redraw panel
            Panel.Invalidate();
        }

        /// <summary>
        /// On resize event
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            // Base call
            base.OnResize(e);

            // Update scroll-bar
            Scrollbar.ForceUpdate();
        }

        /// <summary>
        /// Panel mouse leave
        /// </summary>
        private void OnPanelMouseLeave(object sender, EventArgs e)
        {
            // Update mouse location
            mouseLocation = new Point(-1, -1);

            // Refresh control
            Refresh();

            // Mouse events
            MouseEventArgs args = new MouseEventArgs(MouseButtons.None, 0, Cursor.Position.X, Cursor.Position.Y, 0);

            // Call mouse leave on temp item
            if (_tempItem != null)
                _tempItem.TriggerMouseLeave(this, args);

            // Reset temporary item
            _tempItem = null;
        }

        /// <summary>
        /// Mouse move
        /// </summary>
        private void OnPanelMouseMove(object sender, MouseEventArgs e)
        {
            // Show scroll-bar
            Scrollbar.Wake();

            // Update mouse location
            mouseLocation = new Point(e.Location.X - Panel.AutoScrollPosition.X, e.Location.Y - Panel.AutoScrollPosition.Y);

            // Handle M-Items events
            // Draw items
            if (DisplayedItems.Count > 0)
            {
                // Iterate over all displayed items
                foreach (MItem item in DisplayedItems)
                {
                    // Modify mouse event arguments
                    MouseEventArgs args = new MouseEventArgs(
                        e.Button,
                        e.Clicks,
                        e.X,
                        Math.Abs(Panel.AutoScrollPosition.Y) + e.Y,
                        e.Delta
                    );

                    // Trigger mouse move
                    item.TriggerMouseMove(this, args);

                    // Trigger MouseEnter and MouseLeave methods of M-Items
                    // Do mouse hover over item?
                    if (item.Bounds.Contains(mouseLocation))
                    {
                        // Didn't we already triggered OnMouseEnter method for this item?
                        if (_tempItem == item)
                            return;

                        // Trigger mouse leave before changing temp-item
                        if (_tempItem != null)
                            _tempItem.TriggerMouseLeave(this, e);

                        // Update currently hovered item
                        _tempItem = item;

                        // Trigger item's OnMouseEnter method
                        item.TriggerMouseEnter(this, e);
                    }
                }
            }

            // Redraw control
            Invalidate(true);

            // Force update component
            Component.Outdated = true;

            // Force update scroll-bar
            Scrollbar.ForceUpdate();

            // Call resize
            OnResize(EventArgs.Empty);

            // Refresh control
            Refresh();

            // Redraw panel
            Panel.Invalidate();
        }

        /// <summary>
        /// Filter items
        /// </summary>
        private void FilterItems()
        {
            // Clear displayed items
            DisplayedItems.Clear();

            // Do we have any items to filter?
            if (Items.Count > 0)
            {
                // Iterate over all items
                foreach (MItem item in Items)
                {
                    // Do primary text contains searched word (filter) ?
                    if (item.PrimaryText.ToLower().Contains(_filter.ToLower()))
                    {
                        // Add item to displayed-items collection
                        DisplayedItems.Add(item);
                    }
                }
            }

            // Redraw control
            Invalidate(true);

            // Force update component
            Component.Outdated = true;

            // Force update scroll-bar
            Scrollbar.ForceUpdate();

            // Call resize
            OnResize(EventArgs.Empty);

            // Refresh control
            Refresh();
        }

        /// <summary>
        /// Scrolls to chosen item
        /// </summary>
        public bool ScrollToItem(MItem item)
        {
            // Do we have more than one item?
            if (DisplayedItems.Count > 0)
            {
                // Iterate over all displayed items
                foreach (MItem searchedItem in DisplayedItems)
                {
                    // Is this item we are looking for?
                    if (searchedItem == item)
                    {
                        // Update scroll-bar value
                        Scrollbar.Value = searchedItem.Bounds.Y;

                        // Force update
                        Scrollbar.ForceUpdate();

                        // Redraw scroll-bar
                        Scrollbar.Invalidate();

                        // Set scroll view to chosen item
                        Panel.AutoScrollPosition = new Point(0, searchedItem.Bounds.Y);

                        // Refresh
                        Refresh();

                        // Success
                        return true;
                    }
                }
            }

            // Fail -> searched item not found
            return false;
        }

        /// <summary>
        /// Returns all items with passed primary text
        /// </summary>
        /// <returns></returns>
        public List<MItem> FindItems(string primaryText)
        {
            // Result list
            List<MItem> result = new List<MItem>();

            // Iterate over all displayed items
            foreach (MItem item in Items)
            {
                // Do item name match with given primary text?
                if (item.PrimaryText == primaryText)
                {
                    // Returns first item
                    result.Add(item);
                }
            }

            // Returns list with primary text matches
            return result;
        }

        /// <summary>
        /// Handles support for arrow movement in list-box
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Process key data
            switch (keyData)
            {
                // Up key
                case Keys.Up:
                    // If MultiSelect is off
                    if (!MultiSelect)
                    {
                        // Get index
                        int index = (DisplayedItems.IndexOf(SelectedItem) - 1 > 0) ? DisplayedItems.IndexOf(SelectedItem) - 1 : 0;

                        // Update selected item
                        SelectedItem = DisplayedItems[index];

                        // Decrement scroll value
                        Scrollbar.Value = SelectedItem.Bounds.Y - (Height / 2) + (SelectedItem.Height / 2);
                    }
                    else
                    {
                        // Scroll top
                        Scrollbar.Value -= 50;
                    }

                    // End
                    break;
                // Down key
                case Keys.Down:
                    // If MultiSelect is off
                    if (!MultiSelect)
                    {
                        // Get index
                        int index = (DisplayedItems.IndexOf(SelectedItem) + 1 < DisplayedItems.Count) ? DisplayedItems.IndexOf(SelectedItem) + 1 : DisplayedItems.Count - 1;

                        // Update selected item
                        SelectedItem = DisplayedItems[index];

                        // Decrement scroll value
                        Scrollbar.Value = SelectedItem.Bounds.Y - (Height / 2) + (SelectedItem.Height / 2);
                    }
                    else
                    {
                        // Scroll down
                        Scrollbar.Value += 50;
                    }

                    // End
                    break;
                // If nothing match - default
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }

            // Used CMD key
            return true;
        }

        /// <summary>
        /// Clears both Items and DisplayedItems collections. Resets selected item property.
        /// </summary>
        public void Clear()
        {
            // Reset selected item
            SelectedItem = null;

            // Clear items
            Items.Clear();

            // Clear displayed items
            DisplayedItems.Clear();

            // Redraw control
            Invalidate(true);

            // Force update component
            Component.Outdated = true;

            // Force update scroll-bar
            Scrollbar.ForceUpdate();

            // Refresh control
            Refresh();
        }
    }
}
