using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Minimal.Internal
{
    internal class ColorExtensions
    {
        /// <summary>
        /// Adds amount to RGB values
        /// </summary>
        public static Color AddRGB(int ammount, Color color)
        {
            int r = 0, g = 0, b = 0;

            if (ammount >= 0)
            {
                if (color.R + ammount < 255) { r = color.R + ammount; } else { r = 255; }
                if (color.G + ammount < 255) { g = color.G + ammount; } else { g = 255; }
                if (color.B + ammount < 255) { b = color.B + ammount; } else { b = 255; }
            }
            else
            {
                if (color.R + ammount > 0) { r = color.R + ammount; } else { r = 0; }
                if (color.G + ammount > 0) { g = color.G + ammount; } else { g = 0; }
                if (color.B + ammount > 0) { b = color.B + ammount; } else { b = 0; }
            }

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Lighten color.
        /// </summary>
        public static Color Fade(byte ammount, Color color, Color background)
        {
            return Mix(Color.FromArgb(255 - ammount, color), background);
        }

        /// <summary>
        /// Returns value of two mixed colors
        /// </summary>
        public static Color Mix(Color foreGround, Color background)
        {
            if (foreGround.A == 0)
                return background;

            if (background.A == 0)
                return foreGround;

            if (foreGround.A == 255)
                return foreGround;

            int Alpha = Convert.ToInt32(foreGround.A);
            int B = Alpha * foreGround.B + (255 - Alpha) * background.B >> 8;
            int G = Alpha * foreGround.G + (255 - Alpha) * background.G >> 8;
            int R = Alpha * foreGround.R + (255 - Alpha) * background.R >> 8;
            int A = foreGround.A;

            if (background.A == 255)
                A = 255;
            if (A > 255)
                A = 255;
            if (R > 255)
                R = 255;
            if (G > 255)
                G = 255;
            if (B > 255)
                B = 255;

            return Color.FromArgb(Math.Abs(R), Math.Abs(G), Math.Abs(B));
        }

        public static int PerceivedBrightness(Color c)
        {
            return (int)Math.Sqrt(
            c.R * c.R * .241 +
            c.G * c.G * .691 +
            c.B * c.B * .068);
        }

        /// <summary>  
        /// Method for changing the opacity of an image  
        /// </summary>  
        public static Image SetImageOpacity(Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {

                    // Create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    // Set the opacity  
                    matrix.Matrix33 = opacity;

                    // Create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    // Set the color (opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    // Now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }

                // Returns image with opacity
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Repaint image with one color
        /// </summary>
        public static Image SetImageColor(Image img, Color c)
        {
            Bitmap b = new Bitmap(img);

            for (int x = 0; x < b.Width; x++)
            {
                for (int y = 0; y < b.Height; y++)
                {
                    b.SetPixel(x, y, Color.FromArgb(b.GetPixel(x, y).A, c));
                }
            }

            return (Image)b;
        }
    }
}
