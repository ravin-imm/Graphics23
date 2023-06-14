using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace A25;

class MyWindow : Window {
   public MyWindow () {
      Width = 800; Height = 600;
      Left = 50; Top = 50;
      WindowStyle = WindowStyle.None;
      Image image = new Image () {
         Stretch = Stretch.None,
         HorizontalAlignment = HorizontalAlignment.Left,
         VerticalAlignment = VerticalAlignment.Top,
      };
      RenderOptions.SetBitmapScalingMode (image, BitmapScalingMode.NearestNeighbor);
      RenderOptions.SetEdgeMode (image, EdgeMode.Aliased);

      mBmp = new WriteableBitmap ((int)Width, (int)Height,
         96, 96, PixelFormats.Gray8, null);
      mStride = mBmp.BackBufferStride;
      image.Source = mBmp;
      Content = image;

      //DrawMandelbrot (-0.5, 0, 1);
      MouseLeftButtonDown += OnMouseDown;
   }

   // Using Bresenham's algorithm
   void DrawLine (int x1, int y1, int x2, int y2) {
      try {
         mBmp.Lock ();
         if (x1 == x2) {
            // Line parallel to Y-axis, simply plot the pixels
            if (y2 < y1) (y1, y2) = (y2, y1);
            for (int y = y1; y <= y2; y++)
               SetPixel (x1, y, 255);
         } else if (y1 == y2) {
            // Line parallel to X-axis, simply plot the pixels
            if (x2 < x1) (x1, x2) = (x2, x1);
            for (int x = x1; x <= x2; x++)
               SetPixel (x, y1, 255);
         } else {
            if (Math.Abs (y2 - y1) < Math.Abs (x2 - x1)) { // Slope is positive
               if (x1 > x2) DrawLinePos (x2, y2, x1, y1);
               else DrawLinePos (x1, y1, x2, y2);
            } else {
               if (y1 > y2) DrawLineNeg (x2, y2, x1, y1);
               else DrawLineNeg (x1, y1, x2, y2);
            }
         }
         mBmp.AddDirtyRect (new Int32Rect (Math.Min (x1, x2), Math.Min (y1, y2), Math.Abs (x1 - x2), Math.Abs (y1 - y2)));
      } finally {
         mBmp.Unlock ();
      }
   }

   void DrawLinePos (int x1, int y1, int x2, int y2) {
      int dx = x2 - x1, dy = y2 - y1, yDelta = 1;
      if (dy < 0) { yDelta = -1; dy = -dy; }
      int diff = 2 * dy - dx; // error
      int y = y1;

      for (int x = x1; x <= x2; x++) {
         SetPixel (x, y, 255);
         if (diff > 0) { y += yDelta; diff -= 2 * dx; } // If the difference is -ve, we add the next one
         diff += 2 * dy;
      }
   }

   void DrawLineNeg (int x1, int y1, int x2, int y2) {
      int dx = x2 - x1, dy = y2 - y1, xDelta = 1;
      if (dx < 0) { xDelta = -1; dx = -dx; }
      int diff = 2 * dx - dy; // error
      int x = x1;

      for (int y = y1; y <= y2; y++) {
         SetPixel (x, y, 255);
         if (diff > 0) { x += xDelta; diff -= 2 * dy; } // If the difference is -ve, we add the next one
         diff += 2 * dx;
      }
   }

   void DrawMandelbrot (double xc, double yc, double zoom) {
      try {
         mBmp.Lock ();
         mBase = mBmp.BackBuffer;
         int dx = mBmp.PixelWidth, dy = mBmp.PixelHeight;
         double step = 2.0 / dy / zoom;
         double x1 = xc - step * dx / 2, y1 = yc + step * dy / 2;
         for (int x = 0; x < dx; x++) {
            for (int y = 0; y < dy; y++) {
               Complex c = new Complex (x1 + x * step, y1 - y * step);
               SetPixel (x, y, Escape (c));
            }
         }
         mBmp.AddDirtyRect (new Int32Rect (0, 0, dx, dy));
      } finally {
         mBmp.Unlock ();
      }
   }

   byte Escape (Complex c) {
      Complex z = Complex.Zero;
      for (int i = 1; i < 32; i++) {
         if (z.NormSq > 4) return (byte)(i * 8);
         z = z * z + c;
      }
      return 0;
   }

   void OnMouseMove (object sender, MouseEventArgs e) {
      if (e.LeftButton == MouseButtonState.Pressed) {
         try {
            mBmp.Lock ();
            mBase = mBmp.BackBuffer;
            var pt = e.GetPosition (this);
            int x = (int)pt.X, y = (int)pt.Y;
            SetPixel (x, y, 255);
            mBmp.AddDirtyRect (new Int32Rect (x, y, 1, 1));
         } finally {
            mBmp.Unlock ();
         }
      }
   }

   void OnMouseDown (object sender, MouseEventArgs e) {
      try {
         mBmp.Lock ();
         mBase = mBmp.BackBuffer;
         var pt = e.GetPosition (this);
         int x = (int)pt.X, y = (int)pt.Y;
         if (Nil (mPtPrev)) mPtPrev = pt;
         else {
            DrawLine ((int)mPtPrev.X, (int)mPtPrev.Y, x, y);
            mPtPrev = new (int.MaxValue, int.MaxValue);
         }
      } finally { mBmp.Unlock (); }

      bool Nil (Point pt) => pt.X == int.MaxValue && pt.Y == int.MaxValue;
   }

   void DrawGraySquare () {
      try {
         mBmp.Lock ();
         mBase = mBmp.BackBuffer;
         for (int x = 0; x <= 255; x++) {
            for (int y = 0; y <= 255; y++) {
               SetPixel (x, y, (byte)x);
            }
         }
         mBmp.AddDirtyRect (new Int32Rect (0, 0, 256, 256));
      } finally {
         mBmp.Unlock ();
      }
   }

   void SetPixel (int x, int y, byte gray) {
      unsafe {
         var ptr = (byte*)(mBase + y * mStride + x);
         *ptr = gray;
      }
   }

   WriteableBitmap mBmp;
   int mStride;
   nint mBase;
   Point mPtPrev = new (int.MaxValue, int.MaxValue);
}

internal class Program {
   [STAThread]
   static void Main (string[] args) {
      Window w = new MyWindow ();
      w.Show ();
      Application app = new Application ();
      app.Run ();
   }
}
