using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Minimal.Components.Core;
using Minimal.Components.Items;
using System.Drawing.Drawing2D;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Minimal.Components
{
    /// <summary>
    /// Chart control
    /// </summary>
    public partial class MChart : MBufferedPanel, IMComponent
    {
        /// <summary>
        /// Component object
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        /// Dimensions of the chart
        /// </summary>
        public Size ValueDimensions { get; set; } = new Size(50, 100);

        /// <summary>
        /// Dimensions of the grid
        /// </summary>
        public Size GridDimenstions { get; set; } = new Size(50, 25);

        /// <summary>
        /// Horizontal translation
        /// </summary>
        private float _translateX = 0;

        /// <summary>
        /// Values of the chart
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObservableCollection<ChartValue> Values { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MChart()
        {
            InitializeComponent();

            // Default variables
            Values = new ObservableCollection<ChartValue>();
            Values.CollectionChanged += OnValuesChanged;
        }

        /// <summary>
        /// Render method
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base call
            base.OnPaint(e);

            // Clear background
            e.Graphics.Clear(Component.SourceTheme.COMPONENT_BACKGROUND.Normal);

            // Render border line
            e.Graphics.DrawRectangle(new Pen(Accent), 0, 0, Width - 1, Height - 1);

            // Grid color
            Color gridColor = Color.FromArgb(50, Accent);

            // Transforms
            e.Graphics.ScaleTransform(1.0f, -1.0f);
            e.Graphics.TranslateTransform(0, -ClientRectangle.Height);
            e.Graphics.TranslateTransform(-_translateX, 1);

            // Render grid - vertical
            for (int x = 0; x < Width + _translateX; x++)
            {
                int w = Width / GridDimenstions.Width;

                if (x % w == 0)
                    e.Graphics.DrawLine(new Pen(gridColor), new Point(x, 0), new Point(x, Height));
            }

            // Render grid - horizontal
            for (int y = 0; y < Height; y++)
            {
                int h = Height / GridDimenstions.Height;

                if (y % h == 0)
                    e.Graphics.DrawLine(new Pen(gridColor), new Point(0, y), new PointF(Width + _translateX, y));
            }

            // Graphics path - space under lines
            GraphicsPath path = new GraphicsPath();

            // Last added line second coordinates
            PointF p = new PointF();

            // Turn on anti-aliasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Render values
            for (int i = 0; i < Values.Count; i++)
            {
                // Do we have next item?
                if (i + 1 > Values.Count - 1)
                    // End for
                    break;

                // Get current item
                ChartValue value = Values[i];

                // Create next item instance
                ChartValue next = Values[i + 1];

                // Get next item
                next = Values[i + 1];

                // Dimensions scaling
                float w = (float) Width / (float) ValueDimensions.Width;
                float h = (float) Height / (float) ValueDimensions.Height;

                // Render value lines
                // Draws lines between this and next chart value
                e.Graphics.DrawLine(new Pen(Color.FromArgb(200, Accent), 1), new PointF(value.X * w, value.Y * h), new PointF(next.X * w, next.Y * h));

                // Update points
                p = new PointF(next.X * w, next.Y * h);

                // Add coordinate to path
                path.AddLine(new PointF(value.X * w, value.Y * h), p);
            }

            // Ends path
            path.AddLine(new PointF(p.X, p.Y), new PointF(p.X, 0));
            path.AddLine(new PointF(p.X, 0), new PointF(0, 0));

            // Draws path
            e.Graphics.FillPath(new SolidBrush(Color.FromArgb(20, Accent)), path);

            // Turn off anti-aliasing
            e.Graphics.SmoothingMode = SmoothingMode.None;
        }

        /// <summary>
        /// When Value collection is changed
        /// </summary>
        private void OnValuesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Do we have any values?
            if (Values.Count > 0)
            {
                // Dimensions scaling
                float w = (float) Width / (float) ValueDimensions.Width;
                float h = (float) Height / (float) ValueDimensions.Height;

                // Set translate
                if (Values.Last().X > ValueDimensions.Width)
                    _translateX = (Values.Last().X * w) - Width;
            }

            // Redraw
            Invalidate();
        }

        /// <summary>
        /// On resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            // Base call
            base.OnResize(eventargs);

            // Redraw = re-render grid
            Invalidate();
        }
    }
}
