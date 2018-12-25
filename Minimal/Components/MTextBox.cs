using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Minimal.External;
using Minimal.Themes;
using Minimal.Components.Core;
using Minimal.Internal;

namespace Minimal.Components
{
    [Designer("Minimal.Components.Designer.TextBoxDesigner")]
    public partial class MTextBox : UserControl, IMComponent
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
        /// Text changed event handler
        /// </summary>
        [Category("Property Changed")]
        [Browsable(true)]
        [Description("Fires when the text is changed.")]
        public new event PropertyChangedEventHandler TextChanged;

        /// <summary>
        /// Text property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Appearance")]
        public override string Text
        {
            get { return InnerTextBox.Text; }
            set
            {
                // Update inner text-box's text
                InnerTextBox.Text = value;

                // Trigger text-changed event
                TextChanged?.Invoke(this, new PropertyChangedEventArgs("TextChanged"));

                // Show both hints if text-box is empty
                if (Text == "")
                {
                    if (AutoHideHints)
                        _leftLabel.Visible = _rightLabel.Visible = true;
                }

                // Call resize event
                OnResize(EventArgs.Empty);

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// PasswordChar property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Behavior")]
        public char PasswordChar
        {
            get { return InnerTextBox.PasswordChar; }
            set
            {
                // Update password-char
                InnerTextBox.PasswordChar = value;

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// Multi-line event handler
        /// </summary>
        [Category("Property Changed")]
        [Browsable(true)]
        [Description("Fires when the Multiline is changed.")]
        public event PropertyChangedEventHandler MultilineChanged;

        /// <summary>
        /// Multi-line property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Behavior")]
        public bool Multiline
        {
            get { return InnerTextBox.Multiline; }
            set
            {
                // Update multi-line
                InnerTextBox.Multiline = value;

                // Trigger multi-line changed event
                MultilineChanged?.Invoke(this, new PropertyChangedEventArgs("Multiline"));

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// MaxLength property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Behavior")]
        public int MaxLength
        {
            get { return InnerTextBox.MaxLength; }
            set
            {
                // Update max-length
                InnerTextBox.MaxLength = value;

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// AcceptsReturn property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Behavior")]
        public bool AcceptsReturn
        {
            get { return InnerTextBox.AcceptsReturn; }
            set
            {
                // Update accepts-return
                InnerTextBox.AcceptsReturn = value;

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// AcceptsTab property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Behavior")]
        public bool AcceptsTab
        {
            get { return InnerTextBox.AcceptsTab; }
            set
            {
                // Update accepts-tab
                InnerTextBox.AcceptsTab = value;

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// CharacterCasing property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Behavior")]
        public CharacterCasing CharacterCasing
        {
            get { return InnerTextBox.CharacterCasing; }
            set
            {
                // Update character-casing
                InnerTextBox.CharacterCasing = value;

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// HideSelection event handler
        /// </summary>
        [Category("Property Changed")]
        [Browsable(true)]
        [Description("Fires when the HideSelection is changed.")]
        public event PropertyChangedEventHandler HideSelectionChanged;

        /// <summary>
        /// HideSelection property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Behavior")]
        public bool HideSelection
        {
            get { return InnerTextBox.HideSelection; }
            set
            {
                // Update hide-selection
                InnerTextBox.HideSelection = value;

                // Trigger hide-selection changed event
                HideSelectionChanged?.Invoke(this, new PropertyChangedEventArgs("HideSelection"));

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// ReadOnly event handler
        /// </summary>
        [Category("Property Changed")]
        [Browsable(true)]
        [Description("Fires when the ReadOnly is changed.")]
        public event PropertyChangedEventHandler ReadOnlyChanged;

        /// <summary>
        /// ReadOnly property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Behavior")]
        public bool ReadOnly
        {
            get { return InnerTextBox.ReadOnly; }
            set
            {
                // Update read-only
                InnerTextBox.ReadOnly = value;

                // Update read-only changed event
                ReadOnlyChanged?.Invoke(this, new PropertyChangedEventArgs("ReadOnly"));

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// ShortcutsEnabled property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Behavior")]
        public bool ShortcutsEnabled
        {
            get { return InnerTextBox.ShortcutsEnabled; }
            set
            {
                // Update shortcuts-enabled
                InnerTextBox.ShortcutsEnabled = value;

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// UseSystemPasswordChar property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Behavior")]
        public bool UseSystemPasswordChar
        {
            get { return InnerTextBox.UseSystemPasswordChar; }
            set
            {
                // Update Use-system-password-char
                InnerTextBox.UseSystemPasswordChar = value;

                // Redraw
                Invalidate();
            }
        }

        /// <summary>
        /// WordWrap property
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Category("Behavior")]
        public bool WordWrap
        {
            get { return InnerTextBox.WordWrap; }
            set
            {
                // Update word-wrap
                InnerTextBox.WordWrap = value;

                // WordWrap
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
        /// Private left hint variable
        /// </summary>
        private string _leftHint;

        /// <summary>
        /// Left hint property
        /// </summary>
        [Category("Minimal")]
        [Description("Text-box's inner left-sided hint.")]
        public string LeftHint
        {
            get { return _leftHint; }
            set
            {
                // Update value
                _leftHint = value;

                // Update text of label
                _leftLabel.Text = _leftHint;
            }
        }

        /// <summary>
        /// Private right hint variable
        /// </summary>
        private string _rightHint;

        /// <summary>
        /// Right hint property
        /// </summary>
        [Category("Minimal")]
        [Description("Text-box's inner right-sided hint.")]
        public string RightHint
        {
            get { return _rightHint; }
            set
            {
                // Update value
                _rightHint = value;

                // Update text of label
                _rightLabel.Text = _rightHint;
            }
        }

        /// <summary>
        /// Private font hint variable
        /// </summary>
        private Font _hintFont;

        /// <summary>
        /// Hint font property
        /// </summary>
        [Category("Minimal")]
        [Description("Font of the both hints.")]
        public Font HintFont
        {
            get { return _hintFont; }
            set
            {
                // Update value
                _hintFont = value;

                // Update text of label
                _rightLabel.Font = _leftLabel.Font = _hintFont;
            }
        }

        /// <summary>
        /// Hides both hints if text-box gets focus.
        /// </summary>
        [Category("Minimal")]
        [Description("True if both hints should hide on focus or when text-box contains text.")]
        public bool AutoHideHints { get; set; }

        /// <summary>
        /// Left label for hint
        /// </summary>
        private MLabel _leftLabel;

        /// <summary>
        /// Right label for hint
        /// </summary>
        private MLabel _rightLabel;

        /// <summary>
        /// Control padding
        /// </summary>
        private int _padding;

        /// <summary>
        /// Accent of the text
        /// </summary>
        private Color _textAccent;

        /// <summary>
        /// Tint alpha
        /// </summary>
        private byte _accentAlpha;

        /// <summary>
        /// Mouse position
        /// </summary>
        private Point _mouse;

        /// <summary>
        /// Inner fake TextBox
        /// </summary>
        internal TextBox InnerTextBox;

        /// <summary>
        /// True if TextBox should have left border
        /// </summary>
        internal bool drawLeftBorder = true;

        /// <summary>
        /// True if TextBox should have right border
        /// </summary>
        internal bool drawRightBorder = true;

        /// <summary>
        /// True if TextBox should have right border
        /// </summary>
        internal bool drawTopBorder = true;

        /// <summary>
        /// True if TextBox should have bottom border
        /// </summary>
        internal bool drawBottomBorder = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public MTextBox()
        {
            // Register a new component
            ComponentManager.RegisterComponent(this);

            // Initialize control
            InitializeComponent();

            // Default variables
            _padding = 10;
            _textAccent = Hex.blue.ToColor();
            _accentAlpha = 0;
            DoubleBuffered = true;

            // Initialize textBox
            InnerTextBox = new TextBox();
            InnerTextBox.Location = new Point(_padding, _padding);
            InnerTextBox.BorderStyle = BorderStyle.None;
            InnerTextBox.Multiline = false;
            InnerTextBox.TextAlign = HorizontalAlignment.Left;
            InnerTextBox.BackColor = Color.White;
            InnerTextBox.TextChanged += new EventHandler(OnTextChanged);
            InnerTextBox.Width = Width - (_padding * 2);
            InnerTextBox.Font = new Font("Segoe UI", 9);
            InnerTextBox.GotFocus += new EventHandler(OnInnerTextBoxGotFocus);
            InnerTextBox.LostFocus += new EventHandler(OnInnerTextBoxLostFocus);

            // Initialize left label
            _leftLabel = new MLabel();
            _leftLabel.Click += OnHintLabelsClick;
            _leftLabel.MouseHover += OnLabelHover;
            _leftLabel.AutoSize = true;
            _leftLabel.Type = LabelType.Alternate;
            Controls.Add(_leftLabel);

            // Initialize right label
            _rightLabel = new MLabel();
            _rightLabel.Click += OnHintLabelsClick;
            _rightLabel.MouseHover += OnLabelHover;
            _rightLabel.AutoSize = true;
            _rightLabel.Type = LabelType.Alternate;
            Controls.Add(_rightLabel);

            // Other
            Height = InnerTextBox.Height + (_padding * 2);
            Controls.Add(InnerTextBox);
            Text = "";
        }

        /// <summary>
        /// On label hover
        /// </summary>
        private void OnLabelHover(object sender, EventArgs e)
        {
            // Set cursor
            Cursor = Cursors.IBeam;
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
        /// Update
        /// </summary>
        private void UpdateComponent(object sender, EventArgs e)
        {
            // Color of text
            _textAccent = ColorExtensions.Mix(Color.FromArgb(_accentAlpha, Component.Accent), Component.SourceTheme.COMPONENT_FOREGROUND.Normal.ToColor());
            InnerTextBox.ForeColor = _textAccent;

            // Alpha
            if (InnerTextBox.Focused)
            {
                if (_accentAlpha < 255)
                    _accentAlpha += 15;
            }
            else
            {
                if (_accentAlpha > 0)
                    _accentAlpha -= 15;
            }

            // Redraw
            Invalidate();
        }

        /// <summary>
        /// Draw
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base painting
            base.OnPaint(e);

            // Get graphics context
            Graphics g = e.Graphics;

            // Client rectangle
            Rectangle rc = ClientRectangle;

            // Clear control
            g.Clear(Parent.BackColor);

            // Update background color of labels
            _leftLabel.BackColor = InnerTextBox.BackColor;
            _rightLabel.BackColor = InnerTextBox.BackColor;

            // Fill color
            Color fill = Enabled ? Component.SourceTheme.COMPONENT_BACKGROUND.Normal.ToColor() : Component.SourceTheme.COMPONENT_BACKGROUND.Disabled.ToColor();

            // TextBox background color
            InnerTextBox.BackColor = fill;

            // Control outline colors
            Color backgroundColor = fill;

            // Fill background
            using (SolidBrush backroundBrush = new SolidBrush(backgroundColor))
                g.FillRectangle(backroundBrush, rc);

            // Draw control outline
            Color frameColor = ColorExtensions.Mix(Color.FromArgb(_accentAlpha, Enabled ? Component.Accent : Component.SourceTheme.COMPONENT_BORDER.Disabled.ToColor()), Enabled ? Component.SourceTheme.COMPONENT_BORDER.Normal.ToColor() : Component.SourceTheme.COMPONENT_BORDER.Disabled.ToColor());

            // Draw borders
            using (Pen framePen = new Pen(frameColor))
            {
                // Top border
                if (drawTopBorder)
                    g.DrawLine(framePen, new Point(0, 0), new Point(Width, 0));

                // Bottom border
                if (drawBottomBorder)
                    g.DrawLine(framePen, new Point(0, Height - 1), new Point(Width, Height - 1));

                // Left border
                if (drawLeftBorder)
                    g.DrawLine(framePen, new Point(0, 0), new Point(0, Height - 1));

                // Right border
                if (drawRightBorder)
                    g.DrawLine(framePen, new Point(Width - 1, 0), new Point(Width - 1, Height - 1));
            }
        }

        /// <summary>
        /// Inner TextBox text-change
        /// </summary>
        protected void OnTextChanged(object sender, EventArgs e)
        {
            // Invoke inner text-box text-changed event
            TextChanged?.Invoke(this, new PropertyChangedEventArgs("TextChanged"));
        }

        /// <summary>
        /// TextBox resize
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            // Base call
            base.OnResize(e);

            // Left label
            _leftLabel.Location = new Point(_padding, _padding);
            int leftOffset = _leftLabel.Visible ? _leftLabel.Width : 0;

            // Right label
            _rightLabel.Location = new Point(Width - _padding - _rightLabel.Width, _padding);
            int rightOffset = _rightLabel.Visible ? _rightLabel.Width : 0;

            // Size of text-box and control
            InnerTextBox.Location = new Point(_padding + leftOffset, _padding);
            InnerTextBox.Width = Width - leftOffset - rightOffset - (_padding * 2);

            // Update height
            if (!InnerTextBox.Multiline)
                Height = InnerTextBox.Height + (_padding * 2);
            else
                InnerTextBox.Height = Height - (_padding * 2);
        }

        /// <summary>
        /// Pass focus to textBox
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            // Base call
            base.OnGotFocus(e);

            // Focus inner text-box
            InnerTextBox.Focus();

            // Reset selection length
            InnerTextBox.SelectionLength = 0;

            // Update control
            Component.Outdated = true;
        }

        /// <summary>
        /// Inner text-box got focus
        /// </summary>
        private void OnInnerTextBoxGotFocus(object sender, EventArgs e)
        {
            // Update control
            Component.Outdated = true;

            // Should hints hide?
            if (AutoHideHints)
            {
                _leftLabel.Visible = _rightLabel.Visible = false;
            }

            // Call resize event
            OnResize(EventArgs.Empty);
        }

        /// <summary>
        /// Inner text-box lost focus
        /// </summary>
        private void OnInnerTextBoxLostFocus(object sender, EventArgs e)
        {
            // Is hiding hints enabled?
            if (AutoHideHints)
            {
                // Show both hints if text-box is empty
                if (Text == "")
                    _leftLabel.Visible = _rightLabel.Visible = true;
            }

            // Call resize event
            OnResize(EventArgs.Empty);
        }

        /// <summary>
        /// After click pass focus to textBox
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Base call
            base.OnMouseDown(e);

            // Focus inner text-box
            InnerTextBox.Focus();

            // Mouse position update
            _mouse = new Point(e.X, e.Y);
        }

        /// <summary>
        /// Set cursor icon
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Base call
            base.OnMouseMove(e);

            // Check if mouse is in position of the control
            if (ClientRectangle.Contains(new Point(_mouse.X, _mouse.Y)))
            {
                // Set cursor
                // Cursor.Current = Cursors.IBeam;
            }
        }

        /// <summary>
        /// Left label click
        /// </summary>
        private void OnHintLabelsClick(object sender, EventArgs e)
        {
            // Pass focus to this control
            Focus();
        }

        /// <summary>
        /// Font changed
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnFontChanged(EventArgs e)
        {
            // Base call
            base.OnFontChanged(e);

            // Size of text-box and control
            InnerTextBox.Location = new Point(_padding, _padding);
            InnerTextBox.Width = Width - (_padding * 2);

            // Update height based due to font size change
            if (!InnerTextBox.Multiline)
                Height = InnerTextBox.Height + (_padding * 2);
            else
                InnerTextBox.Height = Height - (_padding * 2);

            // Redraw control
            Invalidate();
        }

        /// <summary>
        /// Clears Text property of text-box
        /// </summary>
        public void Clear()
        {
            Text = "";
        }
    }
}
