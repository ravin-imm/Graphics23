//// PolyFill.cs - Contains the PolyFill class which is mainly used to 
//// fill a given polygon
//// ---------------------------------------------------------------------------------------
namespace GrayBMP;

#region class PolyFill ---------------------------------------------------------
/// <summary>Class to read a given polygon and to fill it </summary>
class PolyFill {
   public void AddLine (int x0, int y0, int x1, int y1) {
      if (y0 == y1) return; // Don't add horizontal lines
      // We will try to keep Y0 as the smaller one always
      if (y0 > y1) { (y0, y1) = (y1, y0); (x0, x1) = (x1, x0); }
      mLines.Enqueue (new Line (x0, y0, x1, y1));
   }

   public void Fill (GrayBMP bmp, int color) {
      // Current active lines along with their slopes for the given scan and the intersections with the current scanline
      var (aLines, ixs) = (new List<(Line Ln, double Slope)> (), new List<int> ());
      for (int y = 0; y < bmp.Height; y++) {
         ixs.Clear ();
         var ys = y + 0.5;
         // Remove lines that are out of the scope of this scan
         for (int i = aLines.Count - 1; i >= 0; i--)
            if (aLines[i].Ln.Y1 < ys) aLines.RemoveAt (i);
         // Add lines that come into the scope of this scan
         while (true) {
            if (mLines.IsEmpty) break; // Nothing to add, break;
            var ln = mLines.Peek ();
            if (ln.Y0 < ys)
               aLines.Add ((mLines.Dequeue (), (double)(ln.X1 - ln.X0) / (ln.Y1 - ln.Y0)));
            else break;
         }

         if (!aLines.Any ()) continue; // No active lines, continue
         foreach (var (ln, slope) in aLines) 
            ixs.Add ((int)(ln.X1 + slope * (ys - ln.Y1)));

         var (nC, ya) = (ixs.Count, bmp.Height - y);
         if (nC % 2 != 0) throw new Exception ();
         ixs.Sort ();
         for (int i = 0; i < ixs.Count; i += 2)
            bmp.DrawHLine (ixs[i], ixs[i + 1], ya, color);
      }
   }

   /// <summary>Queue to store the lines</summary>
   /// Used the one implemeneted in A.15 assignment
   PriorityQueue<Line> mLines = new ();
}

record Line (int X0, int Y0, int X1, int Y1) : IComparable<Line> {
   /// <summary>Compares two Lines</summary>
   public int CompareTo (Line b) {
      int n = Y0.CompareTo (b.Y0); if (n != 0) return n;   
      return X0.CompareTo (b.X0);
   }
}
#endregion


