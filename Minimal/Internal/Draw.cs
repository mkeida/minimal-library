using System.Drawing;
using System.Drawing.Drawing2D;

namespace Minimal.Internal
{
    /// <summary>
    /// Internal Draw class. Helps with drawing.
    /// </summary>
    internal class Draw
    {
        /// <summary>
        /// Returns ellipse. Zero position is center instead of top left corner.
        /// </summary>
        public static GraphicsPath GetEllipsePath(Point center, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2));

            // Returns our path
            return path;
        }

        /// <summary>
        /// Returns square. Zero position is center instead of top left corner.
        /// </summary>
        public static GraphicsPath GetSquarePath(Point center, int a)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new Rectangle(center.X - a, center.Y - a, a * 2, a * 2));

            // Returns our path
            return path;
        }

        /// <summary>
        /// Returns rectangle. Zero position is center instead of top left corner.
        /// </summary>
        public static GraphicsPath GetRectanglePath(Point center, int a, int b)
        {
            // Find the x-coordinate of the upper-left corner of the rectangle to draw.
            int x = center.X - a / 2;

            // Find y-coordinate of the upper-left corner of the rectangle to draw. 
            int y = center.Y - b / 2;

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new Rectangle(x, y, a, b));
            return path;
        }

        /// <summary>
        /// Use a series of pens with decreasing widths and increasing opacity to draw a GraphicsPath.
        /// </summary>
        public static void FuzzyPath(GraphicsPath path, Graphics g, Color baseColor, int maxOpacity, int width, int opaqueWidth)
        {
            // Number of pens we will uses
            int stepsNum = width - opaqueWidth + 1;

            // Change in alpha between pens
            float delta = (float)maxOpacity / stepsNum / stepsNum;

            // Initial alpha.
            float alpha = delta;

            for (int thickness = width; thickness >= opaqueWidth; thickness--)
            {
                Color color = Color.FromArgb((int)alpha, baseColor.R, baseColor.G, baseColor.B);

                using (Pen pen = new Pen(color, thickness))
                {
                    pen.EndCap = LineCap.Round;
                    pen.StartCap = LineCap.Round;
                    g.DrawPath(pen, path);
                }

                alpha += delta;
            }
        }

        /// <summary>
        /// Returns path of rounded rectangle
        /// </summary>
        public static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
        {
            // Variables
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            // Returns plain rectangle if corner radius is zero
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // Top left arc  
            path.AddArc(arc, 180, 90);

            // Top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            // Closes path
            path.CloseFigure();

            // Returns path
            return path;
        }
    }
}
