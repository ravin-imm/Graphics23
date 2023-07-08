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
      mLines.Add (new Line (x0, y0, x1, y1));
   }

   public void Fill (GrayBMP bmp, int color) {
      // Sort the lines in descending order based on their start y coordinate as
      // it is easy to remove the last one after adding it into the active list.
      mLines.Sort ((a, b) => b.CompareTo (a));
      // Current active lines along with their slopeInvs for the given scan and the intersections with the current scanline
      var (aLines, ixs) = (new List<(Line Ln, double SlopeInv)> (), new List<int> ());
      for (int y = 0; y < bmp.Height; y++) {
         ixs.Clear ();
         var ys = y + 0.5;
         // Remove lines that are out of the scope of this scan
         for (int i = aLines.Count - 1; i >= 0; i--)
            if (aLines[i].Ln.Y1 < ys) aLines.RemoveAt (i);
         // Add lines that come into the scope of this scan
         while (mLines.Any () && mLines[^1].Y0 < ys) { // We sorted in descending order. So pick the last one
            var ln = mLines[^1];
            aLines.Add ((ln, (double)(ln.X1 - ln.X0) / (ln.Y1 - ln.Y0)));
            mLines.RemoveAt (mLines.Count - 1);
         }

         if (!aLines.Any ()) continue; // No active lines, continue
         foreach (var (ln, slopeInv) in aLines) 
            ixs.Add ((int)(ln.X1 + slopeInv * (ys - ln.Y1)));

         var (nC, ya) = (ixs.Count, bmp.Height - y);
         if (nC % 2 != 0) throw new Exception ();
         ixs.Sort ();
         for (int i = 0; i < ixs.Count; i += 2)
            bmp.DrawHLine (ixs[i], ixs[i + 1], ya, color);
      }
   }

   /// <summary>Stores all the lines</summary>
   List<Line> mLines = new ();
}

record Line (int X0, int Y0, int X1, int Y1) : IComparable<Line> {
   /// <summary>Compares two lines and the one with less y component is considered.</summary>
   /// If their y component is same then the x component is considered
   public int CompareTo (Line b) {
      int n = Y0.CompareTo (b.Y0); if (n != 0) return n;   
      return X0.CompareTo (b.X0);
   }
}
#endregion


